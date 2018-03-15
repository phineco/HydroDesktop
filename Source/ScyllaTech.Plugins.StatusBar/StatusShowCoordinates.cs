using DotSpatial.Controls;
using DotSpatial.Controls.Header;

namespace ScyllaTech.Plugins.StatusBar
{
    public class StatusShowCoordinates : Extension
    {
        #region Fields

        private Map _map;
        private StatusPanel _xPanel;
        private StatusPanel _yPanel;


        #endregion

        #region Methods

        /// <inheritdoc />
        public override void Activate()
        {
            _map = (Map)App.Map;
            _map.GeoMouseMove += MapGeoMouseMove;

            _xPanel = new StatusPanel
            {
                Width = 320
            };
            _yPanel = new StatusPanel
            {
                Width = 320
            };
            App.ProgressHandler.Add(_xPanel);
            App.ProgressHandler.Add(_yPanel);

            base.Activate();
        }

        /// <inheritdoc />
        public override void Deactivate()
        {
            _map.GeoMouseMove -= MapGeoMouseMove;

            App.ProgressHandler.Remove(_xPanel);
            App.ProgressHandler.Remove(_yPanel);

            App.HeaderControl.RemoveAll();
            base.Deactivate();
        }

        private void MapGeoMouseMove(object sender, GeoMouseArgs e)
        {
            _xPanel.Caption = string.Format("X: {0:.#####}， Y: {0:.#####}", e.GeographicLocation.X, e.GeographicLocation.Y);
            _yPanel.Caption = string.Format("深圳坐标系 X: {0:.#####}， Y: {0:.#####}", e.GeographicLocation.X, e.GeographicLocation.Y);
        }

        #endregion
    }
}
