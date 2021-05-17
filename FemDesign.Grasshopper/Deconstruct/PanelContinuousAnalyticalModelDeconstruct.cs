// https://strusoft.com/
using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace FemDesign.Grasshopper
{
    public class PanelContinuousAnalyticalModelDeconstruct: GH_Component
    {
       public PanelContinuousAnalyticalModelDeconstruct(): base("PanelContinuousAnalyticalModelDeconstruct.Deconstruct", "Deconstruct", "Deconstruct a panel of continuous analytical model element.", "FemDesign", "Deconstruct")
       {

       }
       protected override void RegisterInputParams(GH_InputParamManager pManager)
       {
           pManager.AddGenericParameter("Panel", "Panel", "Panel", GH_ParamAccess.item);           
       } 
       protected override void RegisterOutputParams(GH_OutputParamManager pManager)
       {
           pManager.AddTextParameter("Guid", "Guid", "Guid.", GH_ParamAccess.item);
           pManager.AddBrepParameter("ExtSurface", "ExtSurface", "ExtSurface", GH_ParamAccess.item);
           pManager.AddGenericParameter("Material", "Material", "Material", GH_ParamAccess.item);
           pManager.AddGenericParameter("Section", "Section", "Section", GH_ParamAccess.item);
           pManager.AddCurveParameter("ExtEdgeCurves", "ExtEdgeCurves", "ExtEdgeCurves", GH_ParamAccess.list);
           pManager.AddGenericParameter("ExtEdgeConnections", "ExtEdgeConnections", "ExtEdgeConnections", GH_ParamAccess.list);
           pManager.AddVectorParameter("LocalX", "LocalX", "LocalX", GH_ParamAccess.item);
           pManager.AddVectorParameter("LocalY", "LocalY", "LocalY", GH_ParamAccess.item);
           pManager.AddTextParameter("Identifier", "Identifier", "Structural element ID.", GH_ParamAccess.item);     
       }
       protected override void SolveInstance(IGH_DataAccess DA)
       {
            // get input
            FemDesign.Shells.Panel obj = null;
            if (!DA.GetData(0, ref obj))
            {
                return;
            }
            if (obj == null)
            {
                return;
            }

            if (obj.InternalPanels.IntPanels.Count != 1)
            {
                throw new System.ArgumentException("Panel has more than 1 internal panel. Panel analytical model is not of type continuous.");
            }

            // return
            DA.SetData(0, obj.Guid);
            DA.SetData(1, obj.InternalPanels.IntPanels[0].Region.ToRhinoBrep());
            DA.SetData(2, obj.Material);
            DA.SetData(3, obj.Section);
            DA.SetDataList(4, obj.InternalPanels.IntPanels[0].Region.ToRhinoCurves());
            DA.SetDataList(5, obj.InternalPanels.IntPanels[0].Region.GetEdgeConnections());
            DA.SetData(6, obj.LocalX.ToRhino());
            DA.SetData(7, obj.LocalY.ToRhino());
            DA.SetData(8, obj.Identifier);
       }
       protected override System.Drawing.Bitmap Icon
       {
           get
           {
                return null;
           }
       }
       public override Guid ComponentGuid
       {
           get { return new Guid("2aef7b29-a024-4591-98c9-5bbce2acb954"); }
       }
    }
}