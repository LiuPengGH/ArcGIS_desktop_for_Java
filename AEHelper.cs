using ESRI.ArcGIS.Analyst3D;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MapScene365.AEHelper
{
    public class MapWorkspace
    {
        public static IWorkspace FWorkspace2D = null;
        public static IWorkspace FWorkspace3D = null;

        public static bool Compact()
        {
            try
            {
                IDatabaseCompact pDBCompact = FWorkspace2D as IDatabaseCompact;
                if (pDBCompact.CanCompact())
                {
                    pDBCompact.Compact();
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static string m_LogFile = "MapScene365";

        public static IWorkspace OpenMDBWorkspace(string sDBFile)
        {
            IWorkspace workspace = null;
            try
            {
                workspace = new AccessWorkspaceFactoryClass().OpenFromFile(sDBFile, 0);
            }
            catch (Exception)
            {
            }
            return workspace;
        }


        public static IWorkspace OpenFileGDBWorkspace(string FileGDBPath)
        {
            IWorkspace workspace = null;
            try
            {
                workspace = new FileGDBWorkspaceFactoryClass().OpenFromFile(FileGDBPath, 0);
            }
            catch (Exception)
            {
            }
            return workspace;
        }

        public static IFeatureDataset GetFeatureDataset(IWorkspace Workspace, string FeatureDatasetName)
        {
            if (Workspace == null)
            {
                return null;
            }
            try
            {
                return (Workspace as IFeatureWorkspace).OpenFeatureDataset(FeatureDatasetName);
            }
            catch
            {
                return null;
            }
        }


        public static IFeatureClass GetFeatureClass(IWorkspace Workspace, string FeatureClassName)
        {
            if (Workspace == null)
            {
                return null;
            }
            try
            {
                return (Workspace as IFeatureWorkspace).OpenFeatureClass(FeatureClassName);
            }
            catch
            {
                return null;
            }
        }

        public static bool DeleteDataset(string DatasetName, IWorkspace Workspace)
        {
            if ((Workspace != null) && (DatasetName != string.Empty))
            {
                IFeatureDataset dataset = (Workspace as IFeatureWorkspace).OpenFeatureDataset(DatasetName);
                if (dataset == null)
                {
                    return false;
                }
                if (dataset.CanDelete())
                {
                    try
                    {
                        dataset.Delete();
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        public static bool DeleteFeatureClass(string FeatureClassName, IFeatureDataset FeatureDataset)
        {
            if ((FeatureDataset != null) && (FeatureClassName != string.Empty))
            {
                IFeatureClass class2 = (FeatureDataset.Workspace as IFeatureWorkspace).OpenFeatureClass(FeatureClassName);
                if (class2 == null)
                {
                    return false;
                }
                IDataset dataset = class2 as IDataset;
                if (dataset.CanDelete())
                {
                    try
                    {
                        dataset.Delete();
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        public static IFeatureDataset CreateFeatureDataset(IWorkspace Workspace, string FeatureDatasetName, ISpatialReference pSpatialRef)
        {
            if (Workspace == null)
            {
                return null;
            }
            try
            {
                return (Workspace as IFeatureWorkspace).CreateFeatureDataset(FeatureDatasetName, pSpatialRef);
            }
            catch
            {
                return null;
            }
        }

        public static IFeatureClass CreateAnnotationFeatureClass(string FeatureClassName, IFeatureDataset FeatureDataset, IFeatureClass FeatureClass, IFields Fields, double ReferenceScale, ISymbolCollection SymbolCollection)
        {
            try
            {
                IObjectClassDescription description = new AnnotationFeatureClassDescriptionClass();
                IFeatureClassDescription description2 = description as IFeatureClassDescription;
                IAnnotateLayerPropertiesCollection annoProperties = new AnnotateLayerPropertiesCollectionClass();
                annoProperties.Clear();
                ILabelEngineLayerProperties2 properties = new MaplexLabelEngineLayerPropertiesClass();
                IMaplexOverposterLayerProperties properties2 = new MaplexOverposterLayerPropertiesClass();
                properties.OverposterLayerProperties = properties2 as IOverposterLayerProperties;
                IAnnotateLayerProperties item = properties as IAnnotateLayerProperties;
                item.FeatureLinked = true;
                annoProperties.Add(item);
                if (Fields.FindField("FLDM") < 0)
                {
                    IField field = new FieldClass();
                    IFieldEdit edit = field as IFieldEdit;
                    edit.Name_2 = "FLDM";
                    edit.AliasName_2 = "FLDM";
                    edit.Type_2 = esriFieldType.esriFieldTypeString;
                    edit.Length_2 = 10;
                    (Fields as IFieldsEdit).AddField(field);
                }
                IFeatureWorkspaceAnno workspace = FeatureDataset.Workspace as IFeatureWorkspaceAnno;
                IGraphicsLayerScale referenceScale = new GraphicsLayerScaleClass
                {
                    ReferenceScale = ReferenceScale,
                    Units = esriUnits.esriMeters
                };
                return workspace.CreateAnnotationClass(FeatureClassName, Fields, description.InstanceCLSID, description.ClassExtensionCLSID, description2.ShapeFieldName, string.Empty, FeatureDataset, FeatureClass, annoProperties, referenceScale, SymbolCollection, true);
            }
            catch
            {
                return null;
            }
        }

        public static bool CreateFeatureClass(IFeatureDataset FeatureDataset, string FeatureClassName, esriFeatureType FeatureType, esriGeometryType GeometryType, IFields Fields, IUID CLSID, IUID CLSEXT, string ConfigWord, ref IFeatureClass FeatureClass)
        {
            if ((FeatureDataset == null) || (FeatureClassName == string.Empty))
            {
                return false;
            }
            FeatureClass = null;
            try
            {
                if (CLSID == null)
                {
                    CLSID = null;
                    CLSID = new UIDClass();
                    switch (FeatureType)
                    {
                        case esriFeatureType.esriFTSimpleJunction:
                            GeometryType = esriGeometryType.esriGeometryPoint;
                            CLSID.Value = "esriGeoDatabase.SimpleJunctionFeature";
                            break;

                        case esriFeatureType.esriFTSimpleEdge:
                            GeometryType = esriGeometryType.esriGeometryPolyline;
                            CLSID.Value = "esriGeoDatabase.SimpleEdgeFeature";
                            break;

                        case esriFeatureType.esriFTComplexJunction:
                            CLSID.Value = "esriGeoDatabase.ComplexJunctionFeature";
                            break;

                        case esriFeatureType.esriFTComplexEdge:
                            GeometryType = esriGeometryType.esriGeometryPolyline;
                            CLSID.Value = "esriGeoDatabase.ComplexEdgeFeature";
                            break;

                        case esriFeatureType.esriFTSimple:
                            CLSID.Value = "esriGeoDatabase.Feature";
                            break;
                    }
                }
                if (Fields == null)
                {
                    IFieldsEdit edit = new FieldsClass();
                    IGeometryDef def = new GeometryDefClass();
                    IGeometryDefEdit edit2 = def as IGeometryDefEdit;
                    edit2.GeometryType_2 = GeometryType;
                    edit2.GridCount_2 = 1;
                    edit2.set_GridSize(0, 10.0);
                    edit2.AvgNumPoints_2 = 2;
                    edit2.HasM_2 = false;
                    edit2.HasZ_2 = false;
                    IField field = new FieldClass();
                    IFieldEdit edit3 = field as IFieldEdit;
                    edit3.Name_2 = "shape";
                    edit3.AliasName_2 = "geometry";
                    edit3.Type_2 = esriFieldType.esriFieldTypeGeometry;
                    edit3.GeometryDef_2 = def;
                    edit.AddField(field);
                    field = new FieldClass();
                    edit3 = field as IFieldEdit;
                    edit3.Name_2 = "OBJECTID";
                    edit3.AliasName_2 = "object identifier";
                    edit3.Type_2 = esriFieldType.esriFieldTypeOID;
                    edit.AddField(field);
                    Fields = edit;
                }
                IClone clone = Fields as IClone;
                IFields fields = clone.Clone() as IFields;
                string shapeFieldName = "shape";
                for (int i = 0; i < fields.FieldCount; i++)
                {
                    IField field2 = fields.get_Field(i);
                    if (field2.Type == esriFieldType.esriFieldTypeGeometry)
                    {
                        shapeFieldName = field2.Name;
                    }
                }
                FeatureClass = FeatureDataset.CreateFeatureClass(FeatureClassName, fields, CLSID as UID, CLSEXT as UID, FeatureType, shapeFieldName, ConfigWord);
                return (FeatureClass != null);
            }
            catch
            {
                return false;
            }
        }

        public static bool CreateFeatureClass(IFeatureDataset FeatureDataset, string FeatureClassName, esriFeatureType FeatureType, esriGeometryType GeometryType, IFields Fields, IUID CLSID, IUID CLSEXT, string ConfigWord, ref IFeatureClass FeatureClass, ref string ErrorMessage)
        {
            if (FeatureDataset == null)
            {
                ErrorMessage = "指定数据集为空。";
                return false;
            }
            if (FeatureClassName == string.Empty)
            {
                ErrorMessage = "指定图层名为空";
                return false;
            }
            FeatureClass = null;
            try
            {
                int num2;
                if (CLSID == null)
                {
                    CLSID = null;
                    CLSID = new UIDClass();
                    switch (FeatureType)
                    {
                        case esriFeatureType.esriFTSimpleJunction:
                            GeometryType = esriGeometryType.esriGeometryPoint;
                            CLSID.Value = "esriGeoDatabase.SimpleJunctionFeature";
                            break;

                        case esriFeatureType.esriFTSimpleEdge:
                            GeometryType = esriGeometryType.esriGeometryPolyline;
                            CLSID.Value = "esriGeoDatabase.SimpleEdgeFeature";
                            break;

                        case esriFeatureType.esriFTComplexJunction:
                            CLSID.Value = "esriGeoDatabase.ComplexJunctionFeature";
                            break;

                        case esriFeatureType.esriFTComplexEdge:
                            GeometryType = esriGeometryType.esriGeometryPolyline;
                            CLSID.Value = "esriGeoDatabase.ComplexEdgeFeature";
                            break;

                        case esriFeatureType.esriFTSimple:
                            CLSID.Value = "esriGeoDatabase.Feature";
                            break;
                    }
                }
                if (Fields == null)
                {

                    IFieldsEdit edit = new FieldsClass();
                    IGeometryDef def = new GeometryDefClass();
                    IGeometryDefEdit edit2 = def as IGeometryDefEdit;
                    edit2.GeometryType_2 = GeometryType;
                    edit2.GridCount_2 = 1;
                    edit2.set_GridSize(0, 10.0);
                    edit2.AvgNumPoints_2 = 2;
                    edit2.HasM_2 = false;
                    edit2.HasZ_2 = false;
                    IField field = new FieldClass();
                    IFieldEdit edit3 = field as IFieldEdit;
                    edit3.Name_2 = "shape";
                    edit3.AliasName_2 = "geometry";
                    edit3.Type_2 = esriFieldType.esriFieldTypeGeometry;
                    edit3.GeometryDef_2 = def;
                    edit.AddField(field);
                    field = new FieldClass();
                    edit3 = field as IFieldEdit;
                    edit3.Name_2 = "OBJECTID";
                    edit3.AliasName_2 = "object identifier";
                    edit3.Type_2 = esriFieldType.esriFieldTypeOID;
                    edit.AddField(field);

                    Fields = edit;
                }
                IClone clone = Fields as IClone;
                IFields fields = clone.Clone() as IFields;
                string shapeFieldName = "shape";
                for (int i = 0; i < fields.FieldCount; i++)
                {
                    IField field2 = fields.get_Field(i);
                    if (field2.Type == esriFieldType.esriFieldTypeGeometry)
                    {
                        shapeFieldName = field2.Name;
                    }
                }
                for (num2 = FeatureClassName.IndexOf('('); num2 >= 0; num2 = FeatureClassName.IndexOf('('))
                {
                    FeatureClassName = FeatureClassName.Substring(0, num2) + "_" + FeatureClassName.Substring(num2 + 1, FeatureClassName.Length - num2);
                }
                for (num2 = FeatureClassName.IndexOf(')'); num2 >= 0; num2 = FeatureClassName.IndexOf(')'))
                {
                    FeatureClassName = FeatureClassName.Substring(0, num2) + "_" + FeatureClassName.Substring(num2 + 1, FeatureClassName.Length - num2);
                }
                FeatureClass = FeatureDataset.CreateFeatureClass(FeatureClassName, fields, CLSID as UID, CLSEXT as UID, FeatureType, shapeFieldName, ConfigWord);
                return (FeatureClass != null);
            }
            catch (Exception exception)
            {
                ErrorMessage = exception.Message + exception.TargetSite;
                return false;
            }
        }

        public static bool CreateFeatureClass(IFeatureWorkspace FeatureWorkspace, string FeatureClassName, esriFeatureType FeatureType, esriGeometryType GeometryType, IFields Fields, IUID CLSID, IUID CLSEXT, string ConfigWord, ref IFeatureClass FeatureClass, ref string ErrorMessage)
        {
            if ((FeatureWorkspace == null) || (FeatureClassName == string.Empty))
            {
                return false;
            }
            FeatureClass = null;
            try
            {
                int num2;
                if (CLSID == null)
                {
                    CLSID = null;
                    CLSID = new UIDClass();
                    switch (FeatureType)
                    {
                        case esriFeatureType.esriFTSimpleJunction:
                            GeometryType = esriGeometryType.esriGeometryPoint;
                            CLSID.Value = "esriGeoDatabase.SimpleJunctionFeature";
                            break;

                        case esriFeatureType.esriFTSimpleEdge:
                            GeometryType = esriGeometryType.esriGeometryPolyline;
                            CLSID.Value = "esriGeoDatabase.SimpleEdgeFeature";
                            break;

                        case esriFeatureType.esriFTComplexJunction:
                            CLSID.Value = "esriGeoDatabase.ComplexJunctionFeature";
                            break;

                        case esriFeatureType.esriFTComplexEdge:
                            GeometryType = esriGeometryType.esriGeometryPolyline;
                            CLSID.Value = "esriGeoDatabase.ComplexEdgeFeature";
                            break;

                        case esriFeatureType.esriFTSimple:
                            CLSID.Value = "esriGeoDatabase.Feature";
                            break;
                    }
                }
                if (Fields == null)
                {
                    IFieldsEdit edit = new FieldsClass();
                    IGeometryDef def = new GeometryDefClass();
                    IGeometryDefEdit edit2 = def as IGeometryDefEdit;
                    edit2.GeometryType_2 = GeometryType;
                    edit2.GridCount_2 = 1;
                    edit2.set_GridSize(0, 10.0);
                    edit2.AvgNumPoints_2 = 2;
                    edit2.HasM_2 = false;
                    edit2.HasZ_2 = false;
                    IField field = new FieldClass();
                    IFieldEdit edit3 = field as IFieldEdit;
                    edit3.Name_2 = "shape";
                    edit3.AliasName_2 = "geometry";
                    edit3.Type_2 = esriFieldType.esriFieldTypeGeometry;
                    edit3.GeometryDef_2 = def;
                    edit.AddField(field);
                    field = new FieldClass();
                    edit3 = field as IFieldEdit;
                    edit3.Name_2 = "OBJECTID";
                    edit3.AliasName_2 = "object identifier";
                    edit3.Type_2 = esriFieldType.esriFieldTypeOID;
                    edit.AddField(field);
                    Fields = edit;
                }
                IClone clone = Fields as IClone;
                IFields fields = clone.Clone() as IFields;
                string shapeFieldName = "shape";
                for (int i = 0; i < fields.FieldCount; i++)
                {
                    IField field2 = fields.get_Field(i);
                    if (field2.Type == esriFieldType.esriFieldTypeGeometry)
                    {
                        shapeFieldName = field2.Name;
                    }
                }
                for (num2 = FeatureClassName.IndexOf('('); num2 >= 0; num2 = FeatureClassName.IndexOf('('))
                {
                    FeatureClassName = FeatureClassName.Substring(0, num2) + "_" + FeatureClassName.Substring(num2 + 1, FeatureClassName.Length - num2);
                }
                for (num2 = FeatureClassName.IndexOf(')'); num2 >= 0; num2 = FeatureClassName.IndexOf(')'))
                {
                    FeatureClassName = FeatureClassName.Substring(0, num2) + "_" + FeatureClassName.Substring(num2 + 1, FeatureClassName.Length - num2);
                }
                FeatureClass = FeatureWorkspace.CreateFeatureClass(FeatureClassName, fields, CLSID as UID, CLSEXT as UID, FeatureType, shapeFieldName, ConfigWord);
                return (FeatureClass != null);
            }
            catch (Exception exception)
            {
                ErrorMessage = exception.Message + exception.TargetSite;
                return false;
            }
        }

        public static bool StartEditingAndOperation(IWorkspace Workspace)
        {
            if (Workspace != null)
            {
                IWorkspaceEdit edit = Workspace as IWorkspaceEdit;
                if (edit.IsBeingEdited())
                {
                    if (!(Workspace as IWorkspaceEdit2).IsInEditOperation)
                    {
                        edit.StartEditOperation();
                    }
                    return true;
                }
                bool pHasEdits = false;
                edit.HasEdits(ref pHasEdits);
                if (pHasEdits)
                {
                    return true;
                }
                try
                {
                    if (Workspace.Type == esriWorkspaceType.esriRemoteDatabaseWorkspace)
                    {
                        if ((Workspace as IVersion2).IsRedefined)
                        {
                            (Workspace as IVersion2).RefreshVersion();
                        }
                        (edit as IMultiuserWorkspaceEdit).StartMultiuserEditing(esriMultiuserEditSessionMode.esriMESMVersioned);
                    }
                    else
                    {
                        edit.StartEditing(true);
                    }
                    if (!(Workspace as IWorkspaceEdit2).IsInEditOperation)
                    {
                        edit.StartEditOperation();
                    }
                    return true;
                }
                catch (Exception exception)
                {
                    CommFun.logFile.LogInfo(m_LogFile, "开启编辑失败：" + exception.Message + exception.TargetSite);
                    return false;
                }
            }
            CommFun.logFile.LogInfo(m_LogFile, "开启编辑失败：工作空间获取为空。");
            return false;
        }

        public static bool SaveOperationAndStopEditing(IWorkspace Workspace, bool ReStartEditingAndOperation)
        {
            if (Workspace == null)
            {
                CommFun.logFile.LogInfo(m_LogFile, "开启编辑失败：工作空间获取为空。");
                return false;
            }
            try
            {
                if (Workspace.Type != esriWorkspaceType.esriRemoteDatabaseWorkspace)
                {
                    IWorkspaceFactoryLockControl workspaceFactory = Workspace.WorkspaceFactory as IWorkspaceFactoryLockControl;
                    if (workspaceFactory.SchemaLockingEnabled)
                    {
                        workspaceFactory.DisableSchemaLocking();
                    }
                }
                if ((Workspace as IWorkspaceEdit2).IsBeingEdited())
                {
                    if ((Workspace as IWorkspaceEdit2).IsInEditOperation)
                    {
                        (Workspace as IWorkspaceEdit).StopEditOperation();
                    }
                    if ((Workspace.Type == esriWorkspaceType.esriRemoteDatabaseWorkspace) && (Workspace is IVersionedWorkspace))
                    {
                        if ((Workspace as IVersionEdit).Reconcile("SDE.DEFAULT"))
                        {
                            CommFun.logFile.LogInfo(m_LogFile, "提交编辑检测到冲突，请检查是否有其他客户端编辑了相同要素。");
                            (Workspace as IWorkspaceEdit).StopEditing(false);
                            return false;
                        }
                        if ((Workspace as IVersionEdit).CanPost())
                        {
                            (Workspace as IVersionEdit).Post("SDE.DEFAULT");
                        }
                    }
                    (Workspace as IWorkspaceEdit).StopEditing(true);
                }
                if (ReStartEditingAndOperation)
                {
                    return StartEditingAndOperation(Workspace);
                }
                return true;
            }
            catch (Exception exception)
            {
                CommFun.logFile.LogInfo(m_LogFile, "停止编辑失败：" + exception.Message + exception.TargetSite);
                return false;
            }
        }

    }

    public class MapFeatureClass
    {
        public static IFeature CreateNewFeature(IFeatureClass pFeaCls, IGeometry pGeo)
        {
            if ((pFeaCls == null) || (pGeo == null))
            {
                return null;
            }
            IFeature feature = null;
            try
            {
                (pGeo as ITopologicalOperator).Simplify();
                feature = pFeaCls.CreateFeature();
                if (feature == null)
                {
                    return null;
                }
                else
                {
                    int index = feature.Fields.FindField("Shape");
                    IGeometryDef pGeometryDef = feature.Fields.get_Field(index).GeometryDef as IGeometryDef;
                    //Z值
                    IZAware pZAware = (IZAware)pGeo;
                    pZAware.ZAware = pGeometryDef.HasZ;
                    if (pGeometryDef.HasZ)
                    {
                        IZ iz1 = (IZ)pGeo;
                        iz1.SetConstantZ(0); //将Z值设置为0
                    }

                    //M值
                    IMAware pMAware = (IMAware)pGeo;
                    pMAware.MAware = pGeometryDef.HasM;
                }
                feature.Shape = pGeo;
                feature.Store();
            }
            catch
            {
            }
            return feature;
        }

        public static IFeature CreateNewFea3D(IFeatureClass pFeaCls, IGeometry pGeo)
        {
            if ((pFeaCls == null) || (pGeo == null))
            {
                return null;
            }
            IFeature feature = null;
            try
            {
                (pGeo as ITopologicalOperator).Simplify();
                feature = pFeaCls.CreateFeature();
                feature.Shape = pGeo;
                feature.Store();
            }
            catch
            {
            }
            return feature;
        }


        public static IFeature CreateNewFeature(IFeatureLayer FeatureLayer, IGeometry Geometry, IRow SourceRow)
        {
            if ((FeatureLayer == null) || (Geometry == null))
            {
                return null;
            }
            IFeature feature = null;
            try
            {
                (Geometry as ITopologicalOperator).Simplify();
                IFeatureClass featureClass = FeatureLayer.FeatureClass;
                if (featureClass != null)
                {
                    feature = featureClass.CreateFeature();
                    if (feature == null)
                    {
                        return null;
                    }
                    feature.Shape = Geometry;
                    if (SourceRow != null)
                    {
                        IRow targetRow = feature;
                        MapFeature.CopyFieldValue(SourceRow, ref targetRow);
                    }
                    feature.Store();
                }
            }
            catch
            {
            }
            return feature;
        }

        public static IFeature GetFeature(IPoint Point, IFeatureClass FeatureClass)
        {
            try
            {
                ISpatialFilter filter = new SpatialFilterClass
                {
                    SearchOrder = esriSearchOrder.esriSearchOrderAttribute,
                    SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects,
                    GeometryField = "SHAPE",
                    Geometry = Point
                };
                IFeatureCursor o = FeatureClass.Search(filter, false);
                IFeature feature = o.NextFeature();
                Marshal.ReleaseComObject(o);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                return feature;
            }
            catch
            {
                return null;
            }
        }

        public static IFeature GetFeature(IPoint Point, IFeatureClass FeatureClass, string SubFields, string WhereClause)
        {
            try
            {
                ISpatialFilter filter = new SpatialFilterClass
                {
                    SearchOrder = esriSearchOrder.esriSearchOrderAttribute,
                    SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects,
                    GeometryField = "SHAPE",
                    Geometry = Point,
                    SubFields = SubFields,
                    WhereClause = WhereClause
                };
                IFeatureCursor o = FeatureClass.Search(filter, false);
                IFeature feature = o.NextFeature();
                Marshal.ReleaseComObject(o);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                return feature;
            }
            catch
            {
                return null;
            }
        }

        public static IFeatureCursor GetFeatureCursor(IFeatureClass FeatureClass, IGeometry Geometry, string WhereClause, esriSpatialRelEnum SpatialRelation, esriSearchOrder SearchOrder, bool IsRecycling)
        {
            if (FeatureClass == null)
            {
                return null;
            }
            if (((Geometry == null) || Geometry.IsEmpty) && string.IsNullOrEmpty(WhereClause))
            {
                return null;
            }
            IFeatureCursor cursor = null;
            ISpatialFilter filter = new SpatialFilterClass();
            try
            {
                if (!((Geometry == null) || Geometry.IsEmpty))
                {
                    filter.Geometry = Geometry;
                    (Geometry as ITopologicalOperator).Simplify();
                    filter.SpatialRel = SpatialRelation;
                }
                filter.WhereClause = WhereClause;
                filter.SearchOrder = SearchOrder;
                filter.GeometryField = FeatureClass.ShapeFieldName;
                cursor = FeatureClass.Search(filter, IsRecycling);
            }
            catch
            {
                cursor = null;
            }
            finally
            {
                Marshal.ReleaseComObject(filter);
            }
            return cursor;
        }

        public static IList<IFeature> GetFeatures(IFeatureClass FeatureClass, string WhereClause)
        {
            IList<IFeature> list2;
            IList<IFeature> list = new List<IFeature>();
            if (FeatureClass == null)
            {
                return list;
            }
            IWorkspace workspace = (FeatureClass as IDataset).Workspace;
            if (workspace.Type == esriWorkspaceType.esriRemoteDatabaseWorkspace)
            {
                WhereClause = WhereClause.Replace("*", "%");
            }
            else if (workspace.Type == esriWorkspaceType.esriLocalDatabaseWorkspace)
            {
                WhereClause = WhereClause.Replace("%", "*");
            }
            IFeatureCursor o = null;
            try
            {
                if (string.IsNullOrEmpty(WhereClause))
                {
                    o = FeatureClass.Search(null, false);
                }
                else
                {
                    IQueryFilter filter = new QueryFilterClass
                    {
                        WhereClause = WhereClause
                    };
                    o = FeatureClass.Search(filter, false);
                }
                IFeature item = o.NextFeature();
                while (item != null)
                {
                    if (item.Shape == null)
                    {
                        item = o.NextFeature();
                    }
                    else
                    {
                        list.Add(item);
                        item = o.NextFeature();
                    }
                }
                list2 = list;
            }
            catch (Exception)
            {
                list2 = list;
            }
            finally
            {
                Marshal.ReleaseComObject(o);
            }
            return list2;
        }

        public static IList<IFeature> GetFeatures(IGeometry Geometry, IFeatureClass FeatureClass, esriSpatialRelEnum SpatialRelation)
        {
            IList<IFeature> list2;
            IList<IFeature> list = new List<IFeature>();
            if ((Geometry == null) || (FeatureClass == null))
            {
                return list;
            }
            IFeatureCursor o = null;
            try
            {
                ISpatialFilter filter = new SpatialFilterClass
                {
                    Geometry = Geometry,
                    GeometryField = FeatureClass.ShapeFieldName,
                    SearchOrder = esriSearchOrder.esriSearchOrderSpatial,
                    SpatialRel = SpatialRelation
                };
                o = FeatureClass.Search(filter, false);
                IFeature item = o.NextFeature();
                while (item != null)
                {
                    if (item.Shape == null)
                    {
                        item = o.NextFeature();
                    }
                    else
                    {
                        list.Add(item);
                        item = o.NextFeature();
                    }
                }
                list2 = list;
            }
            catch (Exception)
            {
                list2 = list;
            }
            finally
            {
                Marshal.ReleaseComObject(o);
            }
            return list2;
        }

        /// <summary>
        /// 将FeatClass属性表高效率转换成DataTable  
        ///gisrsman.cnblogs.com
        /// </summary>
        /// <param name="featCls">输入的要素类</param>
        /// <param name="pQueryFilter">查询器，无则为Null</param>
        /// <returns></returns>
        public static DataTable FeatClass2DataTable(IFeatureClass featCls, IQueryFilter pQueryFilter)
        {
            DataTable pAttDT = null;
            string pFieldName;
            string pFieldValue;
            DataRow pDataRow;

            if (featCls != null)
            {
                //根据IFeatureClass字段结构初始化一个表结构
                pAttDT = InitTableByFeaCls(featCls);

                ITable pFeatTable = featCls as ITable;
                int pFieldCout = pFeatTable.Fields.FieldCount;
                ICursor pCursor = pFeatTable.Search(pQueryFilter, false);
                IRow pRow = pCursor.NextRow();

                while (pRow != null)
                {
                    pDataRow = pAttDT.NewRow();
                    for (int j = 0; j < pFieldCout; j++)
                    {
                        pFieldValue = pRow.get_Value(j).ToString();
                        pFieldName = pFeatTable.Fields.get_Field(j).Name;
                        if ("SHAPE" == pFieldName.ToUpper())
                            continue;
                        pDataRow[pFieldName] = pFieldValue;
                    }
                    pAttDT.Rows.Add(pDataRow);
                    pRow = pCursor.NextRow();
                }
            }
            return pAttDT;
        }

        //根据FeatureClass获取表格头
        public static DataTable InitTableByFeaCls(IFeatureClass ifc)
        {
            ITable itable = ifc as ITable;
            DataTable tb = new DataTable(); IFields fields = itable.Fields; IField field;
            for (int i = 0; i < fields.FieldCount; i++)
            {
                field = fields.get_Field(i);
                DataColumn column = new DataColumn();
                if ("SHAPE" == field.Name || "Shape" == field.Name)
                {
                    //过滤掉shape数据； 
                    continue;
                }
                column.ColumnName = field.Name; column.Caption = field.AliasName;
                column.DataType = typeof(string); tb.Columns.Add(column);
            } return tb;
        }

        /// <summary>
        /// 删除图层所有要素
        /// </summary>
        /// <returns></returns>
        public static bool DeleteAllFeature(IFeatureClass pFeaCls)
        {
            try
            {
                IList<IFeature> pFeas = GetFeatures(pFeaCls, "");
                foreach (IFeature pFea in pFeas)
                {
                    pFea.Delete();
                }
            }
            catch (Exception)
            {
                return false;
            }

            return IsEmpty(pFeaCls);
        }
        /// <summary>
        /// 是否空图层
        /// </summary>
        /// <param name="FeatureClass"></param>
        /// <returns></returns>
        public static bool IsEmpty(IFeatureClass FeatureClass)
        {
            if ((FeatureClass != null) && (FeatureClass.FeatureCount(null) > 0))
            {
                return false;
            }
            return true;
        }

        public static ISpatialReference GetSpatialReference(IFeatureClass pFeatureClass)
        {
            IGeoDataset pGeoDataset = pFeatureClass as IGeoDataset;
            return pGeoDataset.SpatialReference;

        }

    }

    public class MapFeatureLayer
    {
        public static IList<IFeature> GetFeatures(IFeatureLayer FeatureLayer, string WhereClause)
        {
            IList<IFeature> list2;
            IList<IFeature> list = new List<IFeature>();
            if (FeatureLayer == null)
            {
                return list;
            }
            IWorkspace workspace = (FeatureLayer as IDataset).Workspace;
            if (workspace.Type == esriWorkspaceType.esriRemoteDatabaseWorkspace)
            {
                WhereClause = WhereClause.Replace("*", "%");
            }
            else if (workspace.Type == esriWorkspaceType.esriLocalDatabaseWorkspace)
            {
                WhereClause = WhereClause.Replace("%", "*");
            }
            IFeatureCursor o = null;
            try
            {
                if (string.IsNullOrEmpty(WhereClause))
                {
                    o = FeatureLayer.Search(null, false);
                }
                else
                {
                    IQueryFilter queryFilter = new QueryFilterClass
                    {
                        WhereClause = WhereClause
                    };
                    o = FeatureLayer.Search(queryFilter, false);
                }
                IFeature item = o.NextFeature();
                while (item != null)
                {
                    if (item.Shape == null)
                    {
                        item = o.NextFeature();
                    }
                    else
                    {
                        list.Add(item);
                        item = o.NextFeature();
                    }
                }
                list2 = list;
            }
            catch (Exception)
            {
                list2 = list;
            }
            finally
            {
                Marshal.ReleaseComObject(o);
            }
            return list2;
        }

        public static IList<IFeature> GetFeatures(IGeometry Geometry, IFeatureLayer FeatureLayer, esriSpatialRelEnum SpatialRelation)
        {
            IList<IFeature> list2;
            IList<IFeature> list = new List<IFeature>();
            if ((Geometry == null) || (FeatureLayer == null))
            {
                return list;
            }
            IFeatureCursor o = null;
            try
            {
                ISpatialFilter queryFilter = new SpatialFilterClass
                {
                    Geometry = Geometry,
                    SearchOrder = esriSearchOrder.esriSearchOrderSpatial,
                    SpatialRel = SpatialRelation
                };
                o = FeatureLayer.Search(queryFilter, false);
                IFeature item = o.NextFeature();
                while (item != null)
                {
                    if (item.Shape == null)
                    {
                        item = o.NextFeature();
                    }
                    else
                    {
                        list.Add(item);
                        item = o.NextFeature();
                    }
                }
                list2 = list;
            }
            catch (Exception)
            {
                list2 = list;
            }
            finally
            {
                Marshal.ReleaseComObject(o);
            }
            return list2;
        }

        public static bool LabelFeatureLayer(IFeatureLayer FeatureLayer, string LabelExpress, ITextSymbol pTextSymbol, bool bLabel)
        {

            if (FeatureLayer == null)
            {
                return false;
            }
            if (pTextSymbol == null)
            {
                pTextSymbol = new TextSymbolClass();
                stdole.StdFont pFont;//定义字体吧，不知道
                pFont = new stdole.StdFontClass();
                pFont.Name = "宋体";
                pFont.Size = 11;
                pTextSymbol.Font = pFont as stdole.IFontDisp;
                IRgbColor pRgbClr = new RgbColorClass();
                pRgbClr.Red = 225;
                pRgbClr.Blue = 0;
                pRgbClr.Green = 0;
                pTextSymbol.Color = pRgbClr;
            }
            try
            {
                IAnnotateLayerProperties item = new LabelEngineLayerPropertiesClass
                {
                    FeatureLayer = FeatureLayer
                };
                (item as ILabelEngineLayerProperties).Expression = LabelExpress;
                (item as ILabelEngineLayerProperties).Symbol = pTextSymbol;
                IAnnotateLayerPropertiesCollection annotationProperties = (FeatureLayer as IGeoFeatureLayer).AnnotationProperties;
                annotationProperties.Clear();
                annotationProperties.Add(item);
                (FeatureLayer as IGeoFeatureLayer).DisplayAnnotation = bLabel;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsLayerCanBeEdited(IFeatureLayer FeatureLayer)
        {
            if (FeatureLayer != null)
            {
                IFeatureClass featureClass = FeatureLayer.FeatureClass;
                if (featureClass != null)
                {
                    IWorkspace workspace = (featureClass as IDataset).Workspace;
                    return ((workspace is IWorkspaceEdit) && (workspace as IWorkspaceEdit).IsBeingEdited());
                }
            }
            return false;
        }

        public static IFeature GetFeature(IPoint Point, IFeatureLayer FeatureLayer)
        {
            if ((FeatureLayer == null) || (Point == null))
            {
                return null;
            }
            if (FeatureLayer.FeatureClass == null)
            {
                return null;
            }
            try
            {
                ISpatialFilter queryFilter = new SpatialFilterClass
                {
                    SearchOrder = esriSearchOrder.esriSearchOrderSpatial,
                    SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects,
                    Geometry = Point,
                    GeometryField = FeatureLayer.FeatureClass.ShapeFieldName
                };
                IFeatureCursor o = FeatureLayer.Search(queryFilter, false);
                IFeature feature = o.NextFeature();
                Marshal.ReleaseComObject(o);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                return feature;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static IFeature GetFeature(IPoint Point, IFeatureLayer FeatureLayer, string SubFields, string WhereClause)
        {
            try
            {
                if ((FeatureLayer == null) || (FeatureLayer.FeatureClass == null))
                {
                    return null;
                }
                ISpatialFilter queryFilter = new SpatialFilterClass
                {
                    SearchOrder = esriSearchOrder.esriSearchOrderAttribute,
                    SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects,
                    GeometryField = "SHAPE",
                    Geometry = Point,
                    SubFields = SubFields,
                    WhereClause = WhereClause
                };
                IFeatureCursor o = FeatureLayer.Search(queryFilter, false);
                IFeature feature = o.NextFeature();
                Marshal.ReleaseComObject(o);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                return feature;
            }
            catch
            {
                return null;
            }
        }

        public static string GetChineseDescriptionOfFeatureType(IFeatureLayer FeatureLayer)
        {
            if (FeatureLayer != null)
            {
                IFeatureClass featureClass = FeatureLayer.FeatureClass;
                if (featureClass == null)
                {
                    return string.Empty;
                }
                if ((featureClass.FeatureType == esriFeatureType.esriFTAnnotation) || (featureClass.FeatureType == esriFeatureType.esriFTCoverageAnnotation))
                {
                    return "注记";
                }
                switch (featureClass.ShapeType)
                {
                    case esriGeometryType.esriGeometryPoint:
                    case esriGeometryType.esriGeometryMultipoint:
                        return "点";

                    case esriGeometryType.esriGeometryPolyline:
                    case esriGeometryType.esriGeometryLine:
                        return "线";

                    case esriGeometryType.esriGeometryPolygon:
                        return "面";
                }
            }
            return string.Empty;
        }

    }

    public class MapFeature
    {
        public static bool CopyAnnotationFieldValue(IFeature SourceFeature, ref IFeature TargetFeature)
        {
            if ((SourceFeature == null) || (TargetFeature == null))
            {
                return false;
            }
            try
            {
                for (int i = 0; i < TargetFeature.Fields.FieldCount; i++)
                {
                    IField field = TargetFeature.Fields.get_Field(i);
                    if ((field.Type != esriFieldType.esriFieldTypeGUID) && (field.Type != esriFieldType.esriFieldTypeOID))
                    {
                        int index = SourceFeature.Fields.FindField(field.Name);
                        if (index >= 0)
                        {
                            object obj2 = SourceFeature.get_Value(index);
                            if (field.Editable)
                            {
                                TargetFeature.set_Value(i, obj2.ToString());
                            }
                        }
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool CopyAnnotationFieldValue(IFeature SourceFeature, ref IFeatureBuffer TargetFeatureBuffer)
        {
            if ((SourceFeature == null) || (TargetFeatureBuffer == null))
            {
                return false;
            }
            try
            {
                for (int i = 0; i < TargetFeatureBuffer.Fields.FieldCount; i++)
                {
                    IField field = TargetFeatureBuffer.Fields.get_Field(i);
                    if ((field.Type != esriFieldType.esriFieldTypeGUID) && (field.Type != esriFieldType.esriFieldTypeOID))
                    {
                        int index = SourceFeature.Fields.FindField(field.Name);
                        if (index >= 0)
                        {
                            object obj2 = SourceFeature.get_Value(index);
                            if (field.Editable)
                            {
                                TargetFeatureBuffer.set_Value(i, obj2.ToString());
                            }
                        }
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool CopyFieldValue(IFeature SourceFeature, ref IFeature TargetFeature)
        {
            if ((SourceFeature == null) || (TargetFeature == null))
            {
                return false;
            }
            try
            {
                IFields fields = TargetFeature.Fields;
                IFields fields3 = SourceFeature.Fields;
                for (int i = 0; i < fields.FieldCount; i++)
                {
                    IField field = TargetFeature.Fields.get_Field(i);
                    esriFieldType type = field.Type;
                    if ((((field.Editable && (type != esriFieldType.esriFieldTypeGeometry)) && ((type != esriFieldType.esriFieldTypeOID) && (type != esriFieldType.esriFieldTypeBlob))) && (field.Name.ToLower() != "shape_length")) && (field.Name.ToLower() != "shape_area"))
                    {
                        int index = fields3.FindField(field.Name);
                        if (index >= 0)
                        {
                            object obj2 = SourceFeature.get_Value(index);
                            if (!string.IsNullOrEmpty(obj2.ToString()))
                            {
                                TargetFeature.set_Value(i, obj2);
                                if (type == esriFieldType.esriFieldTypeString)
                                {
                                    int length = field.Length;
                                    if (length < obj2.ToString().Length)
                                    {
                                        TargetFeature.set_Value(i, obj2.ToString().Substring(0, length));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static bool CopyFieldValue(IFeature SourceFeature, ref IFeatureBuffer TargetFeatureBuffer)
        {
            if ((SourceFeature == null) || (TargetFeatureBuffer == null))
            {
                return false;
            }
            try
            {
                IFields fields = TargetFeatureBuffer.Fields;
                IFields fields3 = SourceFeature.Fields;
                for (int i = 0; i < fields.FieldCount; i++)
                {
                    IField field = TargetFeatureBuffer.Fields.get_Field(i);
                    esriFieldType type = field.Type;
                    if ((((field.Editable && (type != esriFieldType.esriFieldTypeGeometry)) && ((type != esriFieldType.esriFieldTypeOID) && (type != esriFieldType.esriFieldTypeBlob))) && (field.Name.ToLower() != "shape_length")) && (field.Name.ToLower() != "shape_area"))
                    {
                        int index = fields3.FindField(field.Name);
                        if (index >= 0)
                        {
                            object obj2 = SourceFeature.get_Value(index);
                            if (!string.IsNullOrEmpty(obj2.ToString()))
                            {
                                TargetFeatureBuffer.set_Value(i, obj2);
                                if (type == esriFieldType.esriFieldTypeString)
                                {
                                    int length = field.Length;
                                    if (length < obj2.ToString().Length)
                                    {
                                        TargetFeatureBuffer.set_Value(i, obj2.ToString().Substring(0, length));
                                    }
                                }
                            }
                            else if (field.IsNullable)
                            {
                                TargetFeatureBuffer.set_Value(i, DBNull.Value);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static bool CopyTableValue(DataTable dtPoints, DataRow SourceRow, ref IFeatureBuffer TargetFeatureBuffer)
        {
            if ((SourceRow == null) || (TargetFeatureBuffer == null))
            {
                return false;
            }
            try
            {
                IFields fields = TargetFeatureBuffer.Fields;

                for (int i = 0; i < fields.FieldCount; i++)
                {
                    IField field = TargetFeatureBuffer.Fields.get_Field(i);
                    esriFieldType type = field.Type;
                    if ((((field.Editable && (type != esriFieldType.esriFieldTypeGeometry)) && ((type != esriFieldType.esriFieldTypeOID) && (type != esriFieldType.esriFieldTypeBlob))) && (field.Name.ToLower() != "shape_length")) && (field.Name.ToLower() != "shape_area"))
                    {
                        if (dtPoints.Columns.Contains(field.Name))
                        {
                            object obj2 = SourceRow[field.Name];
                            if (!string.IsNullOrEmpty(obj2.ToString()))
                            {
                                TargetFeatureBuffer.set_Value(i, obj2);
                                if (type == esriFieldType.esriFieldTypeString)
                                {
                                    int length = field.Length;
                                    if (length < obj2.ToString().Length)
                                    {
                                        TargetFeatureBuffer.set_Value(i, obj2.ToString().Substring(0, length));
                                    }
                                }
                            }
                            else if (field.IsNullable)
                            {
                                TargetFeatureBuffer.set_Value(i, DBNull.Value);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static bool CopyFieldValue(IRow SourceRow, ref IRow TargetRow)
        {
            if ((SourceRow == null) || (TargetRow == null))
            {
                return false;
            }
            try
            {
                IFields fields = TargetRow.Fields;
                for (int i = 0; i < fields.FieldCount; i++)
                {
                    IField field = TargetRow.Fields.get_Field(i);
                    esriFieldType type = field.Type;
                    if ((((field.Editable && (type != esriFieldType.esriFieldTypeGeometry)) && ((type != esriFieldType.esriFieldTypeOID) && (type != esriFieldType.esriFieldTypeBlob))) && (field.Name.ToLower() != "shape_length")) && (field.Name.ToLower() != "shape_area"))
                    {
                        int index = SourceRow.Fields.FindField(field.Name);
                        if (index >= 0)
                        {
                            object obj2 = SourceRow.get_Value(index);
                            if (!string.IsNullOrEmpty(obj2.ToString()))
                            {
                                TargetRow.set_Value(i, obj2);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static bool CopyFieldValue(IRow SourceRow, ref IRowBuffer TargetRowBuffer)
        {
            if ((SourceRow == null) || (TargetRowBuffer == null))
            {
                return false;
            }
            try
            {
                IFields fields = TargetRowBuffer.Fields;
                IFields fields3 = SourceRow.Fields;
                for (int i = 0; i < fields.FieldCount; i++)
                {
                    IField field = TargetRowBuffer.Fields.get_Field(i);
                    esriFieldType type = field.Type;
                    if ((((field.Editable && (type != esriFieldType.esriFieldTypeGeometry)) && ((type != esriFieldType.esriFieldTypeOID) && (type != esriFieldType.esriFieldTypeBlob))) && (field.Name.ToLower() != "shape_length")) && (field.Name.ToLower() != "shape_area"))
                    {
                        int index = fields3.FindField(field.Name);
                        if (index >= 0)
                        {
                            object obj2 = SourceRow.get_Value(index);
                            if (!string.IsNullOrEmpty(obj2.ToString()))
                            {
                                TargetRowBuffer.set_Value(i, obj2);
                                if (type == esriFieldType.esriFieldTypeString)
                                {
                                    int length = field.Length;
                                    if (length < obj2.ToString().Length)
                                    {
                                        TargetRowBuffer.set_Value(i, obj2.ToString().Substring(0, length));
                                    }
                                }
                            }
                            else if (field.IsNullable)
                            {
                                TargetRowBuffer.set_Value(i, DBNull.Value);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static DataTable Feature2DataTable(IFeature pFeature)
        {
            DataTable pAttDT = new DataTable(); ;
            if (pFeature != null)
            {
                IFields pFields = pFeature.Fields;
                DataColumn column = new DataColumn();
                column.ColumnName = "Field"; column.Caption = "字段";
                column.DataType = typeof(string);
                pAttDT.Columns.Add(column);
                DataColumn column1 = new DataColumn();
                column1.ColumnName = "Value"; column1.Caption = "值";
                column1.DataType = typeof(string);
                pAttDT.Columns.Add(column1);

                StringBuilder Info = new StringBuilder();
                for (int i = 0; i < pFields.FieldCount; i++)
                {
                    IField pField = pFields.get_Field(i);
                    if (pField.Type != esriFieldType.esriFieldTypeGeometry)
                    {
                        DataRow pDataRow = pAttDT.NewRow();
                        pDataRow["Field"] = pField.Name;
                        pDataRow["Value"] = pFeature.get_Value(pFields.FindField(pField.Name));
                        pAttDT.Rows.Add(pDataRow);
                    }
                }
            }
            return pAttDT;
        }

        public static IPoint GetCenterPoint(IFeature Feature)
        {
            if (Feature != null)
            {
                IGeometry shapeCopy = Feature.ShapeCopy;
                switch (shapeCopy.GeometryType)
                {
                    case esriGeometryType.esriGeometryPoint:
                        return (shapeCopy as IPoint);

                    case esriGeometryType.esriGeometryPolyline:
                        return (shapeCopy as IPolyline).FromPoint;

                    case esriGeometryType.esriGeometryPolygon:
                        return ((shapeCopy as IPolygon) as IArea).Centroid;
                }
            }
            return null;
        }

        public static string GetValue(IFeature Feature, string FieldName)
        {
            if ((Feature == null) || (FieldName == string.Empty))
            {
                return string.Empty;
            }
            IFields fields = Feature.Fields;
            int index = fields.FindField(FieldName);
            if (index < 0)
            {
                return string.Empty;
            }
            try
            {
                esriFieldType type = fields.get_Field(index).Type;
                return Feature.get_Value(index).ToString().Trim();
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string GetValue(IRow Row, string FieldName)
        {
            if ((Row == null) || (FieldName == string.Empty))
            {
                return string.Empty;
            }
            IFields fields = Row.Fields;
            int index = fields.FindField(FieldName);
            if (index < 0)
            {
                return string.Empty;
            }
            try
            {
                esriFieldType type = fields.get_Field(index).Type;
                return Row.get_Value(index).ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        public static bool SetValue(ref IFeature Feature, string FieldName, string Value)
        {
            if (Feature == null)
            {
                return false;
            }
            IRow row = Feature;
            return SetValue(ref row, FieldName, Value);
        }

        public static bool SetValue(ref IFeatureBuffer FeatureBuffer, string FieldName, string Value)
        {
            if (FeatureBuffer == null)
            {
                return false;
            }
            int index = FeatureBuffer.Fields.FindField(FieldName);
            if (index.Equals(-1))
            {
                return false;
            }
            IField field = FeatureBuffer.Fields.get_Field(index);
            if (!field.Editable)
            {
                return false;
            }
            try
            {
                if (string.IsNullOrEmpty(Value))
                {
                    FeatureBuffer.set_Value(index, DBNull.Value);
                }
                else if ((field as IFieldEdit).Type == esriFieldType.esriFieldTypeString)
                {
                    int length = (field as IFieldEdit).Length;
                    if (Value.Length < length)
                    {
                        FeatureBuffer.set_Value(index, Value);
                    }
                    else
                    {
                        FeatureBuffer.set_Value(index, Value.Substring(0, length));
                    }
                }
                else
                {
                    FeatureBuffer.set_Value(index, Value);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static bool SetValue(ref IRow Row, string FieldName, string Value)
        {
            if (Row == null)
            {
                return false;
            }
            int index = Row.Fields.FindField(FieldName);
            if (index.Equals(-1))
            {
                return false;
            }
            IField field = Row.Fields.get_Field(index);
            if (!field.Editable)
            {
                return false;
            }
            try
            {
                if (string.IsNullOrEmpty(Value))
                {
                    Row.set_Value(index, DBNull.Value);
                }
                else if ((field as IFieldEdit).Type == esriFieldType.esriFieldTypeString)
                {
                    int length = (field as IFieldEdit).Length;
                    if (Value.Length < length)
                    {
                        Row.set_Value(index, Value);
                    }
                    else
                    {
                        Row.set_Value(index, Value.Substring(0, length));
                    }
                }
                else
                {
                    Row.set_Value(index, Value);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static bool SetGeometry(ref IFeatureBuffer pFea, IGeometry pGeo)
        {
            try
            {
                (pGeo as ITopologicalOperator).Simplify();
                int index = pFea.Fields.FindField("Shape");
                IGeometryDef pGeometryDef = pFea.Fields.get_Field(index).GeometryDef as IGeometryDef;

                //Z值
                IZAware pZAware = (IZAware)pGeo;
                pZAware.ZAware = pGeometryDef.HasZ;
                if (pGeometryDef.HasZ)
                {
                    IZ iz1 = (IZ)pGeo;
                    iz1.SetConstantZ(0); //将Z值设置为0
                }

                //M值
                IMAware pMAware = (IMAware)pGeo;
                pMAware.MAware = pGeometryDef.HasM;

                pFea.Shape = pGeo;
            }
            catch (Exception)
            {
                return false;
            }
            return true;

        }


        public static bool SetGeometry(ref IFeature pFea, IGeometry pGeo)
        {
            try
            {
                int index = pFea.Fields.FindField("Shape");
                IGeometryDef pGeometryDef = pFea.Fields.get_Field(index).GeometryDef as IGeometryDef;

                //Z值
                IZAware pZAware = (IZAware)pGeo;
                pZAware.ZAware = pGeometryDef.HasZ;
                if (pGeometryDef.HasZ)
                {
                    IZ iz1 = (IZ)pGeo;
                    iz1.SetConstantZ(0); //将Z值设置为0
                }

                //M值
                IMAware pMAware = (IMAware)pGeo;
                pMAware.MAware = pGeometryDef.HasM;

                pFea.Shape = pGeo;
                pFea.Store();
            }
            catch (Exception)
            {
                return false;
            }
            return true;

        }
    }

    /// <summary>
    /// 字段
    /// </summary>
    public class MapField
    {
        /// <summary>
        /// 获取字段最大值
        /// </summary>
        /// <param name="FeatureClass"></param>
        /// <param name="FieldName"></param>
        /// <returns></returns>
        public static int GetMaxValue(IFeatureClass FeatureClass, string FieldName)
        {
            if (FeatureClass == null)
            {
                return -1;
            }
            int index = FeatureClass.Fields.FindField(FieldName);
            if (index < 0)
            {
                return -1;
            }
            try
            {
                ITableSort sort = new TableSortClass
                {
                    Table = FeatureClass as ITable,
                    Fields = FieldName
                };
                sort.set_Ascending(FieldName, false);
                sort.Sort(null);
                IRow row = sort.Rows.NextRow();
                if (row == null)
                {
                    return -1;
                }
                string s = row.get_Value(index).ToString();
                int result = 0;
                int.TryParse(s, out result);
                return result;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// 获取OID字段名称
        /// </summary>
        /// <param name="Fields"></param>
        /// <returns></returns>
        public static string GetOIDFieldName(IFields Fields)
        {
            if ((Fields == null) || (Fields.FieldCount == 0))
            {
                return string.Empty;
            }
            string str = string.Empty;
            try
            {
                for (int i = 0; i < Fields.FieldCount; i++)
                {
                    IField field = Fields.get_Field(i);
                    if (field.Type.Equals(esriFieldType.esriFieldTypeOID))
                    {
                        return field.Name;
                    }
                }
            }
            catch
            {
                str = string.Empty;
            }
            return str;
        }

    }

    /// <summary>
    /// 图形
    /// </summary>
    public class MapGeometry
    {
        /// <summary>
        /// 创建缓冲区
        /// </summary>
        /// <param name="Geometry"></param>
        /// <param name="Distance"></param>
        /// <returns></returns>
        public static IGeometry BufferGeometry(IGeometry Geometry, double Distance)
        {
            if (Geometry == null)
            {
                return null;
            }
            try
            {
                IGeometry geometry = (Geometry as ITopologicalOperator).Buffer(Distance);
                if (!((geometry == null) || geometry.IsEmpty))
                {
                    (geometry as ITopologicalOperator).Simplify();
                }
                return geometry;
            }
            catch
            {
                return Geometry;
            }
        }

        /// <summary>
        /// 创建多级缓冲区
        /// </summary>
        /// <param name="Geometry"></param>
        /// <param name="InitialBufferDistance"></param>
        /// <param name="MaxBufferCount"></param>
        /// <returns></returns>
        public static IGeometry BufferGeometry(IGeometry Geometry, double InitialBufferDistance, int MaxBufferCount)
        {
            if (Geometry == null)
            {
                return null;
            }
            try
            {
                IGeometry geometry = (Geometry as ITopologicalOperator).Buffer(InitialBufferDistance);
                if (!((geometry == null) || geometry.IsEmpty))
                {
                    (geometry as ITopologicalOperator).Simplify();
                }
                if (((geometry == null) || geometry.IsEmpty) && (MaxBufferCount > 1))
                {
                    double distance = InitialBufferDistance;
                    for (int i = 0; i < MaxBufferCount; i++)
                    {
                        distance -= InitialBufferDistance / ((double)MaxBufferCount);
                        if (Math.Round((double)(InitialBufferDistance * distance), 6) <= 0.0)
                        {
                            break;
                        }
                        geometry = (Geometry as ITopologicalOperator).Buffer(distance);
                        if ((geometry != null) && !geometry.IsEmpty)
                        {
                            (geometry as ITopologicalOperator).Simplify();
                            if (!((geometry == null) || geometry.IsEmpty))
                            {
                                break;
                            }
                        }
                    }
                }
                return geometry;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 复制图形IGeometry
        /// </summary>
        /// <param name="Geometry"></param>
        /// <returns></returns>
        public static IGeometry CloneGeometry(IGeometry pGeo)
        {
            if (pGeo == null)
            {
                return null;
            }
            return ((pGeo as IClone).Clone() as IGeometry);
        }

        /// <summary>
        /// 创建点IPoint
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        public static IPoint CreatePoint(double X, double Y)
        {
            IPoint point = new PointClass();
            point.PutCoords(X, Y);
            return point;
        }

        /// <summary>
        /// 创建点IPoint
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        public static IPoint CreatePoint3D(double X, double Y, double Z)
        {
            IPoint point = new PointClass();
            point.PutCoords(X, Y);
            point.Z = Z;
            return point;
        }

        /// <summary>
        /// 创建直线ILine
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static ILine CreateLine(IPoint from, IPoint to)
        {
            ILine pLine = new LineClass();
            pLine.PutCoords(from, to);
            return pLine;
        }

        /// <summary>
        /// 创建只有一个Path的Polyline
        /// </summary>
        /// <param name="PolylineList"></param>
        /// <returns></returns>
        public static IPolyline CreatePolyline(List<IPoint> PolylineList)
        {
            ISegment pSegment;
            ILine pLine;
            object o = Type.Missing;
            ISegmentCollection pPath = new PathClass();
            for (int i = 0; i < PolylineList.Count - 1; i++)
            {
                pLine = CreateLine(PolylineList[i], PolylineList[i + 1]);
                pSegment = pLine as ISegment;
                pPath.AddSegment(pSegment, ref o, ref o);

            }
            IGeometryCollection pPolyline = new PolylineClass();
            pPolyline.AddGeometry(pPath as IGeometry, ref o, ref o);
            return pPolyline as IPolyline;
        }

        /// <summary>
        /// 创建线IPolyline
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public static IPolyline CreatePolyline(IPath Path)
        {
            IPolyline polyline = new PolylineClass();
            (polyline as IPointCollection).AddPointCollection(Path as IPointCollection);
            (polyline as ITopologicalOperator).Simplify();
            return polyline;
        }

        /// <summary>
        /// 创建面IGeometry
        /// </summary>
        /// <param name="Ring"></param>
        /// <returns></returns>
        public static IGeometry CreatePolygon(IRing Ring)
        {
            if (Ring == null)
            {
                return null;
            }
            try
            {
                ISpatialReference spatialReference = Ring.SpatialReference;
                IPolygon polygon = new PolygonClass();
                object missing = Type.Missing;
                int pointCount = (Ring as IPointCollection).PointCount;
                for (int i = 0; i < pointCount; i++)
                {
                    IPoint inPoint = (Ring as IPointCollection).get_Point(i);
                    (polygon as IPointCollection).AddPoint(inPoint, ref missing, ref missing);
                }
                polygon.Close();
                (polygon as ITopologicalOperator).Simplify();
                polygon.Project(spatialReference);
                return polygon;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 创建面IPolygon
        /// </summary>
        /// <param name="dXMin"></param>
        /// <param name="dXMax"></param>
        /// <param name="dYMin"></param>
        /// <param name="dYMax"></param>
        /// <returns></returns>
        public static IPolygon CreatePolygon(double dXMin, double dXMax, double dYMin, double dYMax)
        {
            try
            {
                IPolygon polygon = new PolygonClass();
                object missing = Type.Missing;
                IPoint inPoint = new PointClass();
                inPoint.PutCoords(dXMin, dYMin);
                (polygon as IPointCollection).AddPoint(inPoint, ref missing, ref missing);
                inPoint = new PointClass();
                inPoint.PutCoords(dXMin, dYMax);
                (polygon as IPointCollection).AddPoint(inPoint, ref missing, ref missing);
                inPoint = new PointClass();
                inPoint.PutCoords(dXMax, dYMax);
                (polygon as IPointCollection).AddPoint(inPoint, ref missing, ref missing);
                inPoint = new PointClass();
                inPoint.PutCoords(dXMax, dYMin);
                (polygon as IPointCollection).AddPoint(inPoint, ref missing, ref missing);
                inPoint = new PointClass();
                inPoint.PutCoords(dXMin, dYMin);
                (polygon as IPointCollection).AddPoint(inPoint, ref missing, ref missing);
                return polygon;
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// 用拉伸的方法创建MultiPatch类型的管线
        /// </summary>
        /// <param name="FromPnt">管线起点坐标</param>
        /// <param name="ToPnt">管线终点坐标</param>
        /// <param name="R">管线半径</param>
        /// <param name="division">管线划分的片数，越大管线越逼近圆柱形</param>
        /// <param name="sr">坐标系统</param>
        /// <returns>生成的MultiPatch管线</returns>
        public static IGeometry ExtrudePipeByCoords(IPoint FromPnt, IPoint ToPnt, double R, int division, ISpatialReference sr)
        {
            object Missing = Type.Missing;
            IPointCollection PntsPG = new PolygonClass();
            IVector3D VectorZ = new Vector3DClass();//Z轴向量
            VectorZ.SetComponents(0, 0, 1);
            IVector3D VectorXOY = new Vector3DClass();
            VectorXOY.SetComponents(1, 0, 0);
            double Angle = 2 * Math.PI / division;

            for (int i = 0; i < division; i++)
            {
                VectorXOY.Rotate(Angle, VectorZ);
                IPoint Pnt = CreatePoint3D(VectorXOY.XComponent, VectorXOY.YComponent, VectorXOY.ZComponent);
                PntsPG.AddPoint(Pnt, ref Missing, ref Missing);
            }

            (PntsPG as IPolygon).Close();//自动闭合Polygon
            double length = GetTwoPntsDistance(FromPnt, ToPnt);//获得要拉伸的长度
            IConstructMultiPatch ConstructPath = new MultiPatchClass();
            MakeZAware(PntsPG as IGeometry);//注意要设置要拉伸的Geometry的ZAware属性为true，否则拉伸不成功
            ConstructPath.ConstructExtrude(length, PntsPG as IGeometry);//在Z轴方向上将Polygong拉伸成管线
            IVector3D VectorPipe = new Vector3DClass();//真实世界中管线的方向
            VectorPipe.SetComponents(ToPnt.X - FromPnt.X, ToPnt.Y - FromPnt.Y, ToPnt.Z - FromPnt.Z);
            double RotateAngle = GetTwoVectorsAngleInRadian(VectorZ, VectorPipe);//获得Z轴向量与真实世界中管线向量的夹角，用作旋转使用
            IVector3D VectorAxis = VectorZ.CrossProduct(VectorPipe) as IVector3D;//旋转轴向量

            ITransform3D transforms3D = ConstructPath as ITransform3D;
            transforms3D.RotateVector3D(VectorAxis, RotateAngle);
            transforms3D.Move3D(FromPnt.X, FromPnt.Y, FromPnt.Z);
            IGeometry GeoPipe3D = ConstructPath as IGeometry;
            GeoPipe3D.SpatialReference = sr;
            return GeoPipe3D;
        }

        public static double GetTwoVectorsAngleInRadian(IVector3D VectorZ, IVector3D VectorPipe)
        {
            return VectorZ.DotProduct(VectorPipe);
        }

        private static double GetTwoPntsDistance(IPoint FromPnt, IPoint ToPnt)
        {
            double dx = Math.Pow(ToPnt.X - FromPnt.X, 2);
            double dy = Math.Pow(ToPnt.Y - FromPnt.Y, 2);
            double dz = Math.Pow(ToPnt.Z - FromPnt.Z, 2);
            double d = Math.Sqrt(dx + dy + dz);
            return d;
        }

        public static void MakeZAware(IGeometry geometry)
        {
            IZAware zAware = geometry as IZAware;
            zAware.ZAware = true;
        }

        /// <summary>
        /// 用TriangleStrip创建MultiPatch类型的管线
        /// </summary>
        /// <param name="FromPnt">管线起点坐标</param>
        /// <param name="ToPnt">管线终点坐标</param>
        /// <param name="dRadius">管线半径</param>
        /// <param name="division">管线划分的片数，越大管线越逼近圆柱形，4片为方管</param>
        /// <param name="sr">坐标系统</param>
        /// <returns>生成的MultiPatch管线</returns>
        public static IGeometry DrawPipeLine3DGeo(IPoint FromPnt, IPoint ToPnt, double dR, int division, double dWidth, double dHeight, ISpatialReference sr)
        {
            if ((FromPnt as IZAware).ZSimple && (ToPnt as IZAware).ZSimple)
            {
                IVector3D VectorPipe = new Vector3DClass();
                VectorPipe.SetComponents(ToPnt.X - FromPnt.X, ToPnt.Y - FromPnt.Y, ToPnt.Z - FromPnt.Z);

                VectorPipe.Normalize();
                IVector3D VectorZ = new Vector3DClass();
                VectorZ.SetComponents(0, 0, 1);
                IVector3D VectorCross = VectorPipe.CrossProduct(VectorZ) as IVector3D;
                if (VectorCross.XComponent == 0 && VectorCross.YComponent == 0 && VectorCross.ZComponent == 0)
                    VectorCross.SetComponents(1, 0, 0);

                double Angle = 2 * Math.PI / division;
                double dRadius = dR;
                if (division == 4)
                {
                    Angle = Math.Atan(dHeight / dWidth);
                    dRadius = Math.Sqrt(dWidth * dWidth + dHeight * dHeight);
                }
                VectorCross.Magnitude = dRadius;
                double dAngle = Angle;

                IGeometryCollection GeoCollection = new MultiPatchClass();
                IPointCollection TriStripPoints = new TriangleStripClass();
                IPointCollection TriStripPoints0 = new TriangleStripClass();
                IPointCollection TriStripPoints1 = new TriangleStripClass();
                IPointCollection TriStripPoints2 = new TriangleStripClass();
                object Missing = Type.Missing;

                for (int i = 0; i < division; i++)
                {
                    if (division == 4)
                    {
                        if (i == 0) dAngle = Angle;
                        else if (i == 1) dAngle = Math.PI - 2 * Angle;
                        else if (i == 2) dAngle = 2 * Angle;
                        else if (i == 3) dAngle = Math.PI - 2 * Angle;
                    }

                    VectorCross.Rotate(dAngle, VectorPipe);//旋转底面上的向量
                    VectorCross.Magnitude = dRadius;
                    IPoint PntA = CreatePoint3D(FromPnt.X + VectorCross.XComponent, FromPnt.Y + VectorCross.YComponent, FromPnt.Z + VectorCross.ZComponent);
                    IPoint PntB = CreatePoint3D(ToPnt.X + VectorCross.XComponent, ToPnt.Y + VectorCross.YComponent, ToPnt.Z + VectorCross.ZComponent);
                    TriStripPoints.AddPoint(PntA, ref Missing, ref Missing);
                    TriStripPoints.AddPoint(PntB, ref Missing, ref Missing);

                    VectorCross.Magnitude = dRadius * 0.85;
                    IPoint PntC = CreatePoint3D(FromPnt.X + VectorCross.XComponent, FromPnt.Y + VectorCross.YComponent, FromPnt.Z + VectorCross.ZComponent);
                    IPoint PntD = CreatePoint3D(ToPnt.X + VectorCross.XComponent, ToPnt.Y + VectorCross.YComponent, ToPnt.Z + VectorCross.ZComponent);
                    TriStripPoints0.AddPoint(PntC, ref Missing, ref Missing);
                    TriStripPoints0.AddPoint(PntD, ref Missing, ref Missing);

                    TriStripPoints1.AddPoint(PntA, ref Missing, ref Missing);
                    TriStripPoints1.AddPoint(PntC, ref Missing, ref Missing);

                    TriStripPoints2.AddPoint(PntB, ref Missing, ref Missing);
                    TriStripPoints2.AddPoint(PntD, ref Missing, ref Missing);
                }
                //内壁
                TriStripPoints0.AddPoint(TriStripPoints0.get_Point(0), ref Missing, ref Missing);
                TriStripPoints0.AddPoint(TriStripPoints0.get_Point(1), ref Missing, ref Missing);
                GeoCollection.AddGeometry(TriStripPoints0 as IGeometry, ref Missing, ref Missing);
                //外壁
                TriStripPoints.AddPoint(TriStripPoints.get_Point(0), ref Missing, ref Missing);
                TriStripPoints.AddPoint(TriStripPoints.get_Point(1), ref Missing, ref Missing);
                GeoCollection.AddGeometry(TriStripPoints as IGeometry, ref Missing, ref Missing);
                //起点面
                TriStripPoints1.AddPoint(TriStripPoints1.get_Point(0), ref Missing, ref Missing);
                TriStripPoints1.AddPoint(TriStripPoints1.get_Point(1), ref Missing, ref Missing);
                GeoCollection.AddGeometry(TriStripPoints1 as IGeometry, ref Missing, ref Missing);
                //终点面
                TriStripPoints2.AddPoint(TriStripPoints2.get_Point(0), ref Missing, ref Missing);
                TriStripPoints2.AddPoint(TriStripPoints2.get_Point(1), ref Missing, ref Missing);
                GeoCollection.AddGeometry(TriStripPoints2 as IGeometry, ref Missing, ref Missing);

                IGeometry GeoPipe3D = GeoCollection as IGeometry;
                GeoPipe3D.SpatialReference = sr;
                return GeoPipe3D;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 判断点是否重叠
        /// </summary>
        /// <param name="Point1"></param>
        /// <param name="Point2"></param>
        /// <param name="Tolerance"></param>
        /// <returns></returns>
        public static bool IsPointsOverlap(IPoint Point1, IPoint Point2, double Tolerance)
        {
            double x = Point1.X;
            double y = Point1.Y;
            double num3 = Point2.X;
            double num4 = Point2.Y;
            return ((Math.Abs((double)(num3 - x)) > Tolerance) || (Math.Abs((double)(num4 - y)) > Tolerance));
        }

    }

    /// <summary>
    /// 拓扑
    /// </summary>
    public class MapTopology
    {
        /// <summary>
        /// 获取拓扑规则中文名称
        /// </summary>
        /// <param name="TopologyRuleType"></param>
        /// <returns></returns>
        public static string GetChineseDescriptionOfTopologyRuleType(esriTopologyRuleType TopologyRuleType)
        {
            string str = string.Empty;
            try
            {
                switch (TopologyRuleType)
                {
                    case esriTopologyRuleType.esriTRTAreaNoGaps:
                        str = "图层内部面和面之间必须没有缝隙";
                        break;

                    case esriTopologyRuleType.esriTRTAreaNoOverlap:
                        str = "图层内部面和面之间必须不重叠";
                        break;

                    case esriTopologyRuleType.esriTRTAreaCoveredByAreaClass:
                        str = "必须被其它图层的面覆盖";
                        break;

                    case esriTopologyRuleType.esriTRTAreaAreaCoverEachOther:
                        str = "面与面重叠";
                        break;

                    case esriTopologyRuleType.esriTRTAreaCoveredByArea:
                        str = "必须在其它图层面的内部";
                        break;

                    case esriTopologyRuleType.esriTRTAreaNoOverlapArea:
                        str = "必须不和其它图层的面重叠";
                        break;

                    case esriTopologyRuleType.esriTRTLineCoveredByAreaBoundary:
                        str = "必须在其它图层面的边缘上";
                        break;

                    case esriTopologyRuleType.esriTRTPointCoveredByAreaBoundary:
                        str = "点必须在其他面的边缘上";
                        break;

                    case esriTopologyRuleType.esriTRTPointProperlyInsideArea:
                        str = "点必须在其它面的内部";
                        break;

                    case esriTopologyRuleType.esriTRTLineNoOverlap:
                        str = "图层内部线不可以重叠";
                        break;

                    case esriTopologyRuleType.esriTRTLineNoIntersection:
                        str = "图层内部线和线之间必须不相交";
                        break;

                    case esriTopologyRuleType.esriTRTLineNoDangles:
                        str = "线必须无摇摆";
                        break;

                    case esriTopologyRuleType.esriTRTLineNoPseudos:
                        str = "线不存在伪节点";
                        break;

                    case esriTopologyRuleType.esriTRTLineCoveredByLineClass:
                        str = "线必须被其他图层覆盖";
                        break;

                    case esriTopologyRuleType.esriTRTLineNoOverlapLine:
                        str = "线必须不被其他图层覆盖";
                        break;

                    case esriTopologyRuleType.esriTRTPointCoveredByLine:
                        str = "点必须在其他线上";
                        break;

                    case esriTopologyRuleType.esriTRTPointCoveredByLineEndpoint:
                        str = "必须在其它图层线的端点";
                        break;

                    case esriTopologyRuleType.esriTRTAreaBoundaryCoveredByLine:
                        str = "边缘必须被其它图层的线覆盖";
                        break;

                    case esriTopologyRuleType.esriTRTAreaBoundaryCoveredByAreaBoundary:
                        str = "面的边界必须被其它图层面边界覆盖";
                        break;

                    case esriTopologyRuleType.esriTRTLineNoSelfOverlap:
                        str = "不能自身重叠";
                        break;

                    case esriTopologyRuleType.esriTRTLineNoSelfIntersect:
                        str = "不能自身相交";
                        break;

                    case esriTopologyRuleType.esriTRTLineNoIntersectOrInteriorTouch:
                        str = "线的端点必须在其它线上";
                        break;

                    case esriTopologyRuleType.esriTRTLineEndpointCoveredByPoint:
                        str = "线的节点必须被其他点覆盖";
                        break;

                    case esriTopologyRuleType.esriTRTAreaContainPoint:
                        str = "面必须包含点";
                        break;

                    case esriTopologyRuleType.esriTRTLineNoMultipart:
                        str = "必须是单线";
                        break;

                    default:
                        str = string.Empty;
                        break;
                }
                return str;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取拓扑规则名称
        /// </summary>
        /// <param name="TopologyRuleTypeString"></param>
        /// <returns></returns>
        public static esriTopologyRuleType GetTopologyRuleTypeByChineseDescription(string TopologyRuleTypeString)
        {
            esriTopologyRuleType type2;
            try
            {
                esriTopologyRuleType esriTRTAny = esriTopologyRuleType.esriTRTAny;
                switch (TopologyRuleTypeString)
                {
                    case "必须在其它图层线的端点":
                        esriTRTAny = esriTopologyRuleType.esriTRTPointCoveredByLineEndpoint;
                        break;

                    case "点必须在其他线上":
                        esriTRTAny = esriTopologyRuleType.esriTRTPointCoveredByLine;
                        break;

                    case "点必须在其它面的内部":
                        esriTRTAny = esriTopologyRuleType.esriTRTPointProperlyInsideArea;
                        break;

                    case "图层内部线不可以重叠":
                        esriTRTAny = esriTopologyRuleType.esriTRTLineNoOverlap;
                        break;

                    case "图层内部线和线之间必须不相交":
                        esriTRTAny = esriTopologyRuleType.esriTRTLineNoIntersection;
                        break;

                    case "必须在其它图层面的边缘上":
                        esriTRTAny = esriTopologyRuleType.esriTRTLineCoveredByAreaBoundary;
                        break;

                    case "不能自身重叠":
                        esriTRTAny = esriTopologyRuleType.esriTRTLineNoSelfOverlap;
                        break;

                    case "不能自身相交":
                        esriTRTAny = esriTopologyRuleType.esriTRTLineNoSelfIntersect;
                        break;

                    case "必须是单线":
                        esriTRTAny = esriTopologyRuleType.esriTRTLineNoMultipart;
                        break;

                    case "线的端点必须在其它线上":
                        esriTRTAny = esriTopologyRuleType.esriTRTLineNoIntersectOrInteriorTouch;
                        break;

                    case "线的节点必须被其他点覆盖":
                        esriTRTAny = esriTopologyRuleType.esriTRTLineEndpointCoveredByPoint;
                        break;

                    case "图层内部面和面之间必须不重叠":
                        esriTRTAny = esriTopologyRuleType.esriTRTAreaNoOverlap;
                        break;

                    case "图层内部面和面之间必须没有缝隙":
                        esriTRTAny = esriTopologyRuleType.esriTRTAreaNoGaps;
                        break;

                    case "必须不和其它图层的面重叠":
                        esriTRTAny = esriTopologyRuleType.esriTRTAreaNoOverlapArea;
                        break;

                    case "必须被其它图层的面覆盖":
                        esriTRTAny = esriTopologyRuleType.esriTRTAreaCoveredByAreaClass;
                        break;

                    case "必须在其它图层面的内部":
                        esriTRTAny = esriTopologyRuleType.esriTRTAreaCoveredByArea;
                        break;

                    case "边缘必须被其它图层的线覆盖":
                        esriTRTAny = esriTopologyRuleType.esriTRTAreaBoundaryCoveredByLine;
                        break;

                    case "线必须不被其他图层覆盖":
                        esriTRTAny = esriTopologyRuleType.esriTRTLineNoOverlapLine;
                        break;

                    case "点必须在其他面的边缘上":
                        esriTRTAny = esriTopologyRuleType.esriTRTPointCoveredByAreaBoundary;
                        break;

                    case "面的边界必须被其它图层面边界覆盖":
                        esriTRTAny = esriTopologyRuleType.esriTRTAreaBoundaryCoveredByAreaBoundary;
                        break;

                    case "面与面重叠":
                        esriTRTAny = esriTopologyRuleType.esriTRTAreaAreaCoverEachOther;
                        break;

                    case "线必须无摇摆":
                        esriTRTAny = esriTopologyRuleType.esriTRTLineNoDangles;
                        break;

                    case "线不存在伪节点":
                        esriTRTAny = esriTopologyRuleType.esriTRTLineNoPseudos;
                        break;

                    case "线必须被其他图层覆盖":
                        esriTRTAny = esriTopologyRuleType.esriTRTLineCoveredByLineClass;
                        break;

                    case "面必须包含点":
                        esriTRTAny = esriTopologyRuleType.esriTRTAreaContainPoint;
                        break;

                    default:
                        esriTRTAny = esriTopologyRuleType.esriTRTAny;
                        break;
                }
                type2 = esriTRTAny;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return type2;
        }


    }



    public class MapSpatial
    {
        /// <summary>  
        /// 根据prj文件创建空间参考  
        /// </summary>  
        /// <param name="strProFile">空间参照文件</param>  
        /// <returns></returns>  
        public static ISpatialReference CreateSpatialReference(string strProFile)
        {
            ISpatialReferenceFactory pSpatialReferenceFactory = new SpatialReferenceEnvironmentClass();
            ISpatialReference pSpatialReference = pSpatialReferenceFactory.CreateESRISpatialReferenceFromPRJFile(strProFile);
            return pSpatialReference;
        }


    }
}
