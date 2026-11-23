using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TS3SimRipper
{
    /// <summary>
    /// Interaction logic for MorphPreview.xaml
    /// </summary>
    public partial class MorphPreview : UserControl
    {
        AxisAngleRotation3D rot_x;
        AxisAngleRotation3D rot_y;
        ScaleTransform3D zoom = new ScaleTransform3D(1, 1, 1);
        Transform3DGroup modelTransform, cameraTransform;
        DirectionalLight DirLight1 = new DirectionalLight();
        PointLight PointLight = new PointLight();
        PerspectiveCamera Camera1 = new PerspectiveCamera();
        Model3DGroup modelGroup = new Model3DGroup();
        Viewport3D myViewport = new Viewport3D();
        GeometryModel3D myBodyMesh = null;

        public MorphPreview()
        {
            InitializeComponent();
            rot_x = new AxisAngleRotation3D(new Vector3D(1, 0, 0), 0);
            rot_y = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0);

            cameraTransform = new Transform3DGroup();
            cameraTransform.Children.Add(zoom);
            modelTransform = new Transform3DGroup();
            modelTransform.Children.Add(new RotateTransform3D(rot_y));
            modelTransform.Children.Add(new RotateTransform3D(rot_x));

            DirLight1.Color = Colors.White;
            DirLight1.Direction = new Vector3D(.5, -.5, -1);
            PointLight.Color = Colors.DimGray;
            PointLight.Position = new Point3D(1d, 1d, 1d);

            Camera1.FarPlaneDistance = 20;
            Camera1.NearPlaneDistance = 0.05;
            Camera1.FieldOfView = 45;
            Camera1.LookDirection = new Vector3D(0, -0.10, -3);
            Camera1.UpDirection = new Vector3D(0, 1, 0);
            ModelVisual3D modelsVisual = new ModelVisual3D();
            modelsVisual.Content = modelGroup;

            myViewport.Camera = Camera1;
            myViewport.Children.Add(modelsVisual);
            myViewport.Height = 550;
            myViewport.Width = 480;
            myViewport.Camera.Transform = cameraTransform;
            this.canvas1.Children.Insert(0, myViewport);

            Canvas.SetTop(myViewport, 0);
            Canvas.SetLeft(myViewport, 0);
            this.Width = myViewport.Width;
            this.Height = myViewport.Height;
        }

        MeshGeometry3D SimMesh(GEOM simgeom, float yOffset)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            Point3DCollection verts = new Point3DCollection();
            Vector3DCollection normals = new Vector3DCollection();
            PointCollection uvs = new PointCollection();
            Int32Collection facepoints = new Int32Collection();
            int indexOffset = 0;

            GEOM g = simgeom;

            for (int i = 0; i < g.numberVertices; i++)
            {
                float[] pos = g.getPosition(i);
                verts.Add(new Point3D(pos[0], pos[1] - (yOffset * .5), pos[2]));
                float[] norm = g.getNormal(i);
                normals.Add(new Vector3D(norm[0], norm[1], norm[2]));
                float[] uv = g.getUV(i, 0);
                uvs.Add(new Point(uv[0], uv[1]));
            }

            for (int i = 0; i < g.numberFaces; i++)
            {
                int[] face = g.getFaceIndices(i);
                facepoints.Add(face[0] + indexOffset);
                facepoints.Add(face[1] + indexOffset);
                facepoints.Add(face[2] + indexOffset);
            }

            indexOffset += g.numberVertices;

            mesh.Positions = verts;
            mesh.TriangleIndices = facepoints;
            mesh.Normals = normals;
            mesh.TextureCoordinates = uvs;
            return mesh;
        }

        internal ImageBrush GetImageBrush(System.Drawing.Image image)
        {
            BitmapImage bmpImg = new BitmapImage();
            MemoryStream ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Position = 0;
            bmpImg.BeginInit();
            bmpImg.StreamSource = ms;
            bmpImg.EndInit();
            ImageBrush img = new ImageBrush();
            img.ImageSource = bmpImg;
            img.Stretch = Stretch.Fill;
            img.TileMode = TileMode.None;
            img.ViewportUnits = BrushMappingMode.Absolute;
            return img;
        }

        public void Start_Mesh(GEOM[] model, System.Drawing.Image[] texture, System.Drawing.Image[] specular, bool setView)
        {
            Cursor = Cursors.Arrow;
            modelGroup.Children.Clear();
            modelGroup.Children.Add(DirLight1);
            modelGroup.Children.Add(PointLight);

            float modelHeight = 0;
            float modelDepth = 0;
            for (int i = 0; i < model.Length; i++)
            {
                float[] tmp = model != null && model[i] != null ? model[i].GetHeightandDepth() : new float[] { 0, 0 };
                if (tmp[0] > modelHeight) modelHeight = tmp[0];
                if (tmp[1] > modelDepth) modelDepth = tmp[1];
            }
            if (setView)
            {
                Camera1.Position = new Point3D(0, 0, modelHeight + (modelDepth * 2));
                sliderXMove.Value = 0;
                sliderYMove.Value = 0;
                sliderZoom.Value = -(modelHeight + (modelDepth * 2));
            }

            for (int i = 0; i < model.Length; i++)
            {
                if (model[i] != null)
                {
                    MaterialGroup myMaterial = new MaterialGroup();
                    if (texture != null && texture.Length > i && texture[i] != null) myMaterial.Children.Add(new DiffuseMaterial(GetImageBrush(texture[i])));
                    else myMaterial.Children.Add(new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(196, 196, 196))));
                    if (specular != null && specular.Length > i && specular[i] != null) myMaterial.Children.Add(new SpecularMaterial(GetImageBrush(specular[i]), 25d));

                    MeshGeometry3D myBody = SimMesh(model[i], modelHeight);
                    myBodyMesh = new GeometryModel3D(myBody, myMaterial);
                    myBodyMesh.Transform = modelTransform;
                    modelGroup.Children.Add(myBodyMesh);
                }
            }
        }

        public void Stop_Mesh()
        {
            modelGroup.Children.Clear();
        }

        private void sliderZoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Camera1.Position = new Point3D(Camera1.Position.X, Camera1.Position.Y, -sliderZoom.Value);
        }

        private void sliderYMove_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Camera1.Position = new Point3D(Camera1.Position.X, -sliderYMove.Value, Camera1.Position.Z);
        }

        private void sliderXMove_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Camera1.Position = new Point3D(-sliderXMove.Value, Camera1.Position.Y, Camera1.Position.Z);
        }

        private void sliderYRot_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            rot_y.Angle = sliderYRot.Value;
        }

        private void sliderXRot_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            rot_x.Angle = sliderXRot.Value;
        }
    }
}
