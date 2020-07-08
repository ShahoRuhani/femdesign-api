// https://strusoft.com/
using System.Collections.Generic;
using System.Xml.Serialization;
#region dynamo
using Autodesk.DesignScript.Runtime;
#endregion

namespace FemDesign.Loads
{
    /// <summary>
    /// loads
    /// </summary>
    [System.Serializable]
    [IsVisibleInDynamoLibrary(false)]
    public class Loads
    {
        // dummy elements are needed to deserialize an .struxml model correctly as order of elements is needed.
        // if dummy elements are not used for undefined types deserialization will not work properly
        // when serializing these dummy elements must be nulled. 
        [XmlElement("point_load", Order = 1)]
        public List<PointLoad> pointLoad = new List<PointLoad>(); // point_load_type
        [XmlElement("line_load", Order = 2)]
        public List<LineLoad> lineLoad = new List<LineLoad>(); // line_load_type
        [XmlElement("pressure_load", Order = 3)]
        public List<PressureLoad> pressureLoad = new List<PressureLoad>(); // pressure_load_type
        [XmlElement("surface_load", Order = 4)]
        public List<SurfaceLoad> surfaceLoad = new List<SurfaceLoad>(); // surface_load_type
        [XmlElement("line_temperature_variation_load", Order = 5)]
        public List<DummyXmlObject> lineTemperatureVariationLoad {get {return null;} set {value = null;}} // line_temperature_load_type
        [XmlElement("surface_temperature_variation_load", Order = 6)]
        public List<DummyXmlObject> surfaceTemperatureVariationLoad {get {return null;} set {value = null;}} // surface_temperature_variation_load
        [XmlElement("line_stress_load", Order = 7)]
        public List<DummyXmlObject> lineStressLoad {get {return null;} set {value = null;}} // line_stress_load
        [XmlElement("surface_stress_load", Order = 8)]
        public List<DummyXmlObject> surfaceStressLoad {get {return null;} set {value = null;}} // surface_stress_load
        [XmlElement("point_support_motion_load", Order = 9)]
        public List<DummyXmlObject> pointSupportMotionLoad {get {return null;} set {value = null;}} // point_support_motion_load_type
        [XmlElement("line_support_motion_load", Order = 10)]
        public List<DummyXmlObject> lineSupportMotionLoad {get {return null;} set {value = null;}} // line_support_motion_load_type
        
        [XmlElement("surface_support_motion_load", Order = 11)]
        public List<DummyXmlObject> surfaceSupportMotionLoad {get {return null;} set {value = null;}} // surface_support_motion_load_type
        
        [XmlElement("mass", Order = 12)]
        public List<DummyXmlObject> mass {get {return null;} set {value = null;}} // mass_point_type
        
        [XmlElement("load_case_mass_conversion_table", Order = 13)]
        public MassConversionTable loadCaseMassConversionTable {get; set;} // mass_conversion_type
        
        [XmlElement("seismic_load", Order = 14)]
        public List<DummyXmlObject> seismicLoad {get {return null;} set {value = null;}} // seismic_load_type
        
        [XmlElement("footfall_analysis_data", Order = 15)]
        public List<DummyXmlObject> footfallAnalysisData {get {return null;} set {value = null;}} // footfall_type
        
        [XmlElement("moving_load", Order = 16)]
        public List<DummyXmlObject> movingLoad {get {return null;} set {value = null;}} // moving_load_type
        
        [XmlElement("load_case", Order = 17)]
        public List<LoadCase> loadCase = new List<LoadCase>(); // load_case_type
        
        [XmlElement("load_combination", Order = 18)]
        public List<LoadCombination> loadCombination = new List<LoadCombination>(); // load_combination_type
        
        [XmlElement("load_group_table", Order = 19)]
        public List<DummyXmlObject> loadGroupTable {get {return null;} set {value = null;}} // load_group_table_type
        
        /// <summary>
        /// Get PointLoad, LineLoad, PressureLoad and SurfaceLoads from Loads.
        /// </summary>
        internal List<object> GetLoads()
        {
            var objs = new List<object>();
            objs.AddRange(this.pointLoad);
            objs.AddRange(this.lineLoad);
            objs.AddRange(this.pressureLoad);
            objs.AddRange(this.surfaceLoad);
            return objs;
        }
        
    }
}