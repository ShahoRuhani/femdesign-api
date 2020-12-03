// https://strusoft.com/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

#region dynamo
using Autodesk.DesignScript.Runtime;
#endregion

namespace FemDesign.Bars
{
    /// <summary>
    /// bar_part_type
    /// 
    /// Underlying representation of a Bar-element.
    /// </summary>
    [System.Serializable]
    [IsVisibleInDynamoLibrary(false)]
    public class BarPart: EntityBase
    {
        /// <summary>
        /// Edge field
        /// </summary>
        [XmlElement("curve", Order = 1)]
        public Geometry.Edge _edge;

        /// <summary>
        /// Edge property
        /// </summary>
        [XmlIgnore]
        public Geometry.Edge Edge
        {
            get
            {
                return this._edge;
            }
            set
            {
                this._edge = value;

                if (this.Type == "beam")
                {
                    if (value.Type == "line" || value.Type == "arc")
                    {
                        this._edge = value;
                    }
                    else
                    {
                        throw new System.ArgumentException($"Edge type: {value.Type}, is not line or arc.");
                    }   
                }
                else if (this.Type == "column")
                {
                    // check if line
                    if (!value.IsLine())
                    {
                        throw new System.ArgumentException($"Edge type: {value.Type}, is not line.");
                    }

                    // check if line is vertical
                    if (!value.IsLineVertical())
                    {
                        throw new System.ArgumentException("Edge (Line) must be vertial for column definition");
                    }

                    // set value
                    this._edge = value;
                }
                else if (this.Type == "truss")
                {
                    // check if line
                    if (!value.IsLine())
                    {
                        throw new System.ArgumentException($"Edge type: {value.Type}, is not line.");
                    }

                    // set value
                    this._edge = value;
                }
                else
                {
                    throw new System.ArgumentException($"Incorrect type of bar: {this.Type}");
                }
            }
        }
        
        [XmlIgnore]
        private Geometry.FdCoordinateSystem _coordinateSystem;

        [XmlIgnore]
        private Geometry.FdCoordinateSystem CoordinateSystem
        {
            get
            {
                if (this._coordinateSystem == null)
                {
                    this._coordinateSystem = this.Edge.CoordinateSystem;
                    return this._coordinateSystem;
                }
                else
                {
                    return this._coordinateSystem;
                }
            }
            set
            {
                this._coordinateSystem = value;
                this._localY = value.LocalY;
            }
        }

        [XmlIgnore]
        public Geometry.FdPoint3d LocalOrigin
        {
            get
            {
                return this.CoordinateSystem.Origin;
            }
        }

        [XmlIgnore]
        public Geometry.FdVector3d LocalX
        {
            get
            {
                return this.CoordinateSystem.LocalX;
            }
        }

        [XmlElement("local-y", Order = 2)]
        public Geometry.FdVector3d _localY;

        [XmlIgnore]
        public Geometry.FdVector3d LocalY
        {
            get 
            { 
                return this._localY;
            }
            set
            {
                this.CoordinateSystem.SetYAroundX(value);
                this._localY = this.CoordinateSystem.LocalY;
            }
        }

        [XmlIgnore]
        public Geometry.FdVector3d LocalZ
        {
            get
            {
                return this.CoordinateSystem.LocalZ;
            }
        }

        /// <summary>
        /// Private field for bar with start and end eccentricity
        /// </summary>
        [XmlIgnore]
        public Eccentricity[] _eccentricities = new Eccentricity[2];

        [XmlIgnore]
        public Eccentricity[] Eccentricities
        {
            get
            {
                return this._eccentricities;
            }
            set
            {
                if (value.Length == 1)
                {
                    this._eccentricities[0] = value[0];
                    this._eccentricities[1] = value[0];
                }
                else if (value.Length == 2)
                {
                    if (this.Type == "truss")
                    {
                        throw new System.ArgumentException("Truss can only have 1 section");
                    }

                    this._eccentricities[0] = value[0];
                    this._eccentricities[1] = value[1];
                }
                else
                {
                    throw new System.ArgumentException($"Incorrect length of Sections: {value.Length}. Length should be 1 or 2");
                }
            }
        }

        /// <summary>
        /// Get/set start eccentricity of bar
        /// </summary>
        [XmlIgnore]
        public Eccentricity StartEccentricity
        {
            get
            {
                return this._eccentricities[0];
            }
            set
            {
                this._eccentricities[0] = value;
            }
        }

        /// <summary>
        /// Get/set end eccentricity of bar
        /// </summary>
        [XmlIgnore]
        public Eccentricity EndEccentricity
        {
            get
            {
                return this._eccentricities[1];
            }
            set
            {
                this._eccentricities[1] = value;
            }
        }

        /// <summary>
        /// Private field for bar with start and end section
        /// </summary>
        [XmlIgnore]
        private Sections.Section[] _sections = new Sections.Section[2];

        [XmlIgnore]
        public Sections.Section[] Sections
        {
            get
            {
                return this._sections;
            }
            set
            {
                if (value.Length == 1)
                {
                    this._sections[0] = value[0];
                    this._sections[1] = value[0];
                }
                else if (value.Length == 2)
                {
                    if (this.Type == "truss")
                    {
                        throw new System.ArgumentException("Truss can only have 1 section");
                    }

                    this._sections[0] = value[0];
                    this._sections[1] = value[1];
                }
                else
                {
                    throw new System.ArgumentException($"Incorrect length of Sections: {value.Length}. Length should be 1 or 2");
                }
            }
        }

        /// <summary>
        /// Check if sections are null
        /// </summary>
        public bool SectionsAreNull
        {
            get
            {
                return (this.Sections[0] == null & this.Sections[1] == null);
            }
        }

        /// <summary>
        /// Try to get a uniform section. Compares section based on guid.
        /// </summary>
        public Sections.Section UniformSection
        {
            get
            {
                if (this.Sections.Length != 2)
                {
                    throw new System.ArgumentException($"Sections should contain 2 items but contains {this.Sections.Length}.");
                }

                if (this.Sections[0].Guid == this.Sections[1].Guid)
                {
                    return this.Sections[0];
                }
                else
                {
                    throw new System.ArgumentException("Sections contain two different sections. Impossible to get a uniform section.");
                }
            }
        }

        /// <summary>
        /// Get/set start section of bar
        /// </summary>
        [XmlIgnore]
        public Sections.Section StartSection
        {
            get
            {
                return this._sections[0];
            }
            set
            {
                // set value
                this._sections[0] = value;
            }
        }

        /// <summary>
        /// Get/set end section of bar
        /// </summary>
        [XmlIgnore]
        public Sections.Section EndSection
        {
            get
            {
                return this._sections[1];
            }
            set
            {
                // set value
                this._sections[1] = value;             
            }
        }

        /// <summary>
        /// Section position field
        /// </summary>
        [XmlIgnore]
        public double[] _sectionPos;

        /// <summary>
        /// Section position property. Set position of sections for complex section by defining the parammetric position 0-1.
        /// </summary>
        [XmlIgnore]
        public double[] SectionPos
        {
            get
            {
                if (this._sectionPos == null)
                {
                    double[] val = new double[this.Sections.Length];

                    // set start and end pos
                    val[0] = 0;
                    val[val.Length - 1] = 1;

                    if (val.Length > 2)
                    {
                        // set intermediate pos
                        for (int idx = 1; idx < val.Length - 1; idx++)
                        {
                            val[idx] = 1/(val.Length - 1)*idx;
                        }
                    }

                    // set
                    this._sectionPos = val;

                    // return
                    return this._sectionPos;
                }

                else
                {
                    // return
                    return this._sectionPos;
                }
            }
            set
            {
                if (value.Length != this.Sections.Length)
                {
                    throw new System.ArgumentException($"Length of value: {value.Length} must be equal to length of Sections: {this.Sections.Length}");
                }

                if (value[0] != 0)
                {
                    throw new System.ArgumentException("First item of value must be 0");
                }

                if (value[value.Length - 1] != 1)
                {
                    throw new System.ArgumentException("Last item of value must be 1");
                }

                this._sectionPos = value;
            }
        }

        /// <summary>
        /// Private property used for complex section updates
        /// </summary>
        [XmlIgnore]
        private Sections.ModelSection[] ModelSection
        {
            get
            {
                return new Sections.ModelSection[]
                {
                    new Sections.ModelSection(0, this.StartSection, this.StartEccentricity),
                    new Sections.ModelSection(1, this.EndSection, this.EndEccentricity)
                };
            }
        }

        /// <summary>
        /// Complex section field.
        /// </summary>
        [XmlIgnore]
        private Sections.ComplexSection _complexSection;

        /// <summary>
        /// Complex section property getter. For model deserialization use other property.
        /// </summary>
        [XmlIgnore]
        public Sections.ComplexSection ComplexSection
        {
            get
            {
                if (this.Type == "truss")
                {
                    return null;
                }
                else
                {
                    try
                    {
                        // update _complexSection with BarPart sections and eccentricities
                        this._complexSection.Section = ModelSection.ToList();
                    }
                    catch (System.NullReferenceException)
                    {
                        // pass
                    }

                    // return
                    return this._complexSection;
                }
            }
            set
            {
                this._complexSection = value;
                this.ComplexSectionRef = this._complexSection.Guid;
            }
        }

        [XmlIgnore]
        public bool ComplexSectionIsNull
        {
            get
            {
                if (this._complexSection == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Material field
        /// </summary>
        [XmlIgnore]
        public Materials.Material _material;

        /// <summary>
        /// Material property. When a new material is set the ComplexMaterialRef is updated.
        /// </summary>
        [XmlIgnore]
        public Materials.Material Material
        {
            get
            {
                return this._material;
            }
            set
            {
                this._material = value;
                this.ComplexMaterialRef = this._material.Guid;
            }
        }

        [XmlAttribute("name")]
        public string _identifier;

        [XmlIgnore]
        public string Identifier
        {
            get
            {
                return this._identifier;
            }
            set
            {
                this._identifier = value + ".1";
            }
        }

        [XmlIgnore]
        private string _type;
        
        [XmlIgnore]
        public string Type
        {
            get {return this._type;}
            set {this._type = RestrictedString.BeamType(value);}
        }

        [XmlAttribute("complex_material")]
        public System.Guid ComplexMaterialRef { get; set; } // guidtype

        [XmlIgnore]
        private System.Guid _complexSectionRef;

        [XmlAttribute("complex_section")]
        public System.Guid ComplexSectionRef
        {
            get
            {
                // used when deserializing only
                if (this.ComplexSectionIsNull && this.Type != "truss")
                {
                    return this._complexSectionRef;
                }

                // used for trusses only
                else if (this.ComplexSectionIsNull && this.Type == "truss")
                {
                    if (!this.SectionsAreNull)
                    {
                        this._complexSectionRef = this.UniformSection.Guid;
                    }
                    return this._complexSectionRef;
                }

                // used for all other cases
                else
                {
                    this._complexSectionRef = this.ComplexSection.Guid;
                    return this._complexSectionRef;
                }
            }
            set
            {
                this._complexSectionRef = value;
            }
        }

        [XmlAttribute("made")]
        public string _made; // steelmadetype

        [XmlIgnore]
        public string Made
        {
            get {return this._made;}
            set
            {
                if (value == null)
                {
                    this._made = value;
                }
                else
                {
                    this._made = RestrictedString.SteelMadeType(value);
                }
            }
        }
        [XmlAttribute("ecc_calc")]
        public bool EccentricityCalc { get; set; } // bool

        [XmlElement("connectivity", Order = 3)]
        public Connectivity[] _connectivities = new Connectivity[2]; // connectivity_type
        
        [XmlIgnore]
        public Connectivity[] Connectivities
        {
            get
            {
                return this._connectivities;
            }
            set
            {
                if (value.Length == 1)
                {
                    this._connectivities[0] = value[0];
                    this._connectivities[1] = value[0];
                }
                else if (value.Length == 2)
                {
                    this._connectivities[0] = value[0];
                    this._connectivities[1] = value[1];
                }
                else
                {
                    throw new System.ArgumentException($"Incorrect length of Connectivities: {value.Length}. Length should be 1 or 2");
                }
            }
        }
           

        [XmlIgnore]  
        private ModelEccentricity _modelEccentricity;

        [XmlElement("eccentricity", Order = 4)]
        public ModelEccentricity ModelEccentricity
        {
            get
            {
                if (this.Type == "truss")
                {
                    return null;
                }
                
                else
                {
                    if (this._modelEccentricity == null)
                    {
                        // create new model eccentricity
                        this._modelEccentricity = new ModelEccentricity(this.Eccentricities[0], this.Eccentricities.Last(), true);
                    }
                    else
                    {
                        // update model eccentricity
                        this._modelEccentricity.StartAnalytical = this.Eccentricities[0];
                        this._modelEccentricity.EndAnalytical = this.Eccentricities.Last();
                        this._modelEccentricity.StartPhysical = this.Eccentricities[0];
                        this._modelEccentricity.EndPhysical = this.Eccentricities.Last();
                    }

                    return this._modelEccentricity;
                } 
            }
            set
            {
                this._modelEccentricity = value;
            }
        }

        [XmlElement("buckling_data", Order = 5)]
        public Buckling.BucklingData BucklingData { get; set; } // buckling_data_type
        [XmlElement("end", Order = 6)]
        public string End = "";

        /// <summary>
        /// Parameterless constructor for serialization.
        /// </summary>
        private BarPart()
        {
            
        }

        private BarPart(string name, Geometry.Edge _edge, Materials.Material _material)
        {
            this.EntityCreated();
            this.Identifier = name;
            this.ComplexMaterialRef = _material.Guid;
            this.EccentricityCalc = true; // default should be false, but is always true since FD15? should be activated if eccentricity is defined
            this._edge = _edge;
            this.LocalY = _edge.CoordinateSystem.LocalY;
        }

        /// <summary>
        /// Construct BarPart (beam or column)
        /// </summary>
        public BarPart(Geometry.Edge edge, string type, Materials.Material material, Sections.Section[] sections, Connectivity[] connectivities, Eccentricity[] eccentricities, string identifier)
        {
            this.EntityCreated();
            this.Type = type;
            this.Edge = edge;
            this.Material = material;
            this.Sections = sections;
            this.Connectivities = connectivities;
            this.Eccentricities = eccentricities;
            this.EccentricityCalc = true;
            this.ComplexSection = new Sections.ComplexSection(this.ModelSection.ToList());
            this.Identifier = identifier;
        }

        /// <summary>
        /// Construct BarPart (truss)
        /// <summary>
        public BarPart(Geometry.Edge edge, string type, Materials.Material material, Sections.Section section, string identifier)
        {
            this.EntityCreated();
            this.Type = type;
            this.Edge = edge;
            this.Material = material;
            this.Sections = new Sections.Section[1]{section};
            this.Identifier = identifier;
        }

        /// <summary>
        /// Orient this object's coordinate system to GCS
        /// <summary>
        public void OrientCoordinateSystemToGCS()
        {
            var cs = this.CoordinateSystem;
            cs.OrientEdgeTypeLcsToGcs();
            this.CoordinateSystem = cs;
        }
    }
}