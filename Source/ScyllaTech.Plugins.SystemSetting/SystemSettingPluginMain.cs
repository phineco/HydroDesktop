using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Controls.Header;
using DotSpatial.Controls.Docking;
using MySql.Data.MySqlClient;
using DotSpatial.Topology;
using ScyllaTech.Plugins.SystemSetting.Properties;

namespace ScyllaTech.Plugins.SystemSetting
{
    class SystemSettingPluginMain : Extension
    {
        #region Private Member Variables

        private RootItem setting_tab = new RootItem("kSettingTab", "系统设置") { SortOrder = -7 };

        private RootItem about_tab = new RootItem("kAboutTab", "关于") { SortOrder = -6 };

        private MapLineLayer _mapLineLayer;

        public SimpleActionItem rbLoadGps { get; private set; }


        #endregion

        #region Private Methods

        private void AddSystemSettingMenu()
        {
            var head = App.HeaderControl;
            //head.Add(rbSelect = new SimpleActionItem(HeaderControl.HomeRootItemKey, Msg.Select_Features, rbSelect_Click) { ToolTipText = Msg.Select_Features_Tooltip, LargeImage = Resources.select_poly_32, GroupCaption = Msg.Area, ToggleGroupKey = Msg.Area, });
            //_searchSettings.AreaSettings.PolygonsChanged += AreaSettings_PolygonsChanged;

            //head.Add(rbDrawBox = new SimpleActionItem(HeaderControl.HomeRootItemKey, Msg.Draw_Rectangle, rbDrawBox_Click) { ToolTipText = Msg.Draw_Box_Tooltip, LargeImage = Resources.Draw_Box_32, SmallImage = Resources.Draw_Box_16, GroupCaption = Msg.Area, ToggleGroupKey = Msg.Area });
            //_searchSettings.AreaSettings.AreaRectangleChanged += Instance_AreaRectangleChanged;

            //head.Add(new SimpleActionItem(HeaderControl.HomeRootItemKey, Msg.Select_By_Attribute, rbAttribute_Click) { GroupCaption = "地图工具", LargeImage = Resources.select_table_32 });

            head.Add(new SimpleActionItem(HeaderControl.HomeRootItemKey, "定位", rbLatLong_Click) { GroupCaption = "查询", LargeImage = Properties.Resources.select_table_32 });

            rbLoadGps = new SimpleActionItem(HeaderControl.HomeRootItemKey, "同步轨迹", rbLoadGps_Click) { GroupCaption = "同步", LargeImage = Resources.select_table_32 };
            head.Add(rbLoadGps);
            head.Add(new SimpleActionItem(HeaderControl.HomeRootItemKey, "同步采集", rbRefreshGps_Click) { GroupCaption = "同步", LargeImage = Resources.select_table_32 });

            head.Add(setting_tab);
            head.Add(new SimpleActionItem("kSettingTab", "重置工程", rbResetProject_Click) { GroupCaption = "工程", LargeImage = Resources.select_table_32 });
            head.Add(new SimpleActionItem("kSettingTab", "坐标系设置", rbCoorSet_Click) { GroupCaption = "设置", LargeImage = Resources.select_table_32 });
            head.Add(about_tab);
            head.Add(new SimpleActionItem("kAboutTab", "关于", rbAbout_Click) { GroupCaption = "关于", LargeImage = Resources.select_table_32 });
            head.Add(new SimpleActionItem("kAboutTab", "退出系统", rbExitSystem_Click) { GroupCaption = "用户", LargeImage = Resources.select_table_32 });
            head.Add(new SimpleActionItem("kAboutTab", "切换用户", rbSwitchUser_Click) { GroupCaption = "用户", LargeImage = Resources.select_table_32 });

        }



        #endregion

        #region IExtension Members

        /// <summary>
        /// Fires when the plugin should become inactive
        /// </summary>
        public override void Deactivate()
        {
            // Remove ribbon tab
            App.HeaderControl.RemoveAll();
            
            base.Deactivate();
        }
        /// <summary>
        /// Activates the HelpTab plugin
        /// </summary>
        public override void Activate()
        {

            AddSystemSettingMenu();
            // This line ensures that "Enabled" is set to true
            base.Activate();
        }


        #endregion

        #region Event Handlers
        void rbRefreshGps_Click(object sender, EventArgs e)
        {
            LinkedList<Coordinate> tmpList = getGpsData();

            //_mapLineLayer.DataSet.AddRow
            var coors = _mapLineLayer.DataSet.GetFeature(0).Coordinates;
            foreach (Coordinate coor in tmpList)
            {
                coors.Add(coor);
            }

            _mapLineLayer.FeatureSet.InitializeVertices();
            //var line = new LineString(list);

            //Reproject.ReprojectPoints(array, new double[] { 0, 0, 0, 0 }, ProjectionInfo.FromEsriString(KnownCoordinateSystems.Geographic.World.WGS1984.ToEsriString()), App.Map.Projection, 0, 2);
            //App.Map.ResetBuffer();
        }

        private int maxId = 0;
        public static string connStr = "server=139.196.126.19;user id=;password=;database=dhdata;port=3306;";
        public static MySqlConnection conn;
        private LinkedList<Coordinate> getGpsData()
        {
            LinkedList<Coordinate> tmplist = new LinkedList<Coordinate>();
            try
            {
                conn = new MySqlConnection(connStr);
                string sqlStr = string.Format("select PointX, PointY, Id from points where Id > {0} order by Id", maxId);
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sqlStr, conn);
                MySqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    Coordinate coor = new Coordinate(sdr.GetDouble(0), sdr.GetDouble(1));
                    maxId = sdr.GetInt16(2);
                    tmplist.AddLast(coor);
                }
            }
            catch (Exception e)
            {

            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }



            return tmplist;
        }

        void rbLoadGps_Click(object sender, EventArgs e)
        {
            var array = new double[8];
            array[0] = 117.25;
            array[1] = 31.83;
            array[2] = 117.2388;
            array[3] = 31.83;
            var coor1 = new Coordinate(array[0], array[1]);
            var coor2 = new Coordinate(array[2], array[3]);
            var coords = new Coordinate[2];
            coords[0] = coor1;
            coords[1] = coor2;

            LinkedList<Coordinate> tmpList = getGpsData();


            var line = new LineString(tmpList);

            //Reproject.ReprojectPoints(array, new double[] { 0, 0, 0, 0 }, ProjectionInfo.FromEsriString(KnownCoordinateSystems.Geographic.World.WGS1984.ToEsriString()), App.Map.Projection, 0, 2);

            var lineFs = new FeatureSet(DotSpatial.Topology.FeatureType.Line);
            _mapLineLayer = new MapLineLayer(lineFs);
            _mapLineLayer.DataSet.AddFeature(line);
            _mapLineLayer.Symbolizer = new DotSpatial.Symbology.LineSymbolizer(Color.FromArgb(0x33, 0x33, 0x33), 1);
            _mapLineLayer.LegendText = "轨迹";
            App.Map.Layers.Add(_mapLineLayer);
            App.Map.ResetBuffer();
            rbLoadGps.Enabled = false;
        }
        private void rbLatLong_Click(object sender, EventArgs e)
        {
            ZoomToCoordinatesDialog dialog = new ZoomToCoordinatesDialog(App.Map);
            dialog.ShowDialog();

        }
        private void rbResetProject_Click(object sender, EventArgs e)
        {


        }
        private void rbCoorSet_Click(object sender, EventArgs e)
        {


        }
        private void rbAbout_Click(object sender, EventArgs e)
        {


        }
        private void rbExitSystem_Click(object sender, EventArgs e)
        {
            

        }
        private void rbSwitchUser_Click(object sender, EventArgs e)
        {

        }
        void aboutButton_Click(object sender, EventArgs e)
        {
            //AboutBox frm = new AboutBox();

            //frm.StartPosition = FormStartPosition.CenterScreen;
            //frm.Show();
        }

        #endregion
    
    }
}
