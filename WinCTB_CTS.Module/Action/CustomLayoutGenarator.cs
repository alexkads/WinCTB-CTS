//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using DevExpress.ExpressApp;
//using DevExpress.ExpressApp.DC;
//using DevExpress.ExpressApp.Editors;
//using DevExpress.ExpressApp.Model;
//using DevExpress.ExpressApp.Model.Core;
//using DevExpress.ExpressApp.Model.NodeGenerators;
//using DevExpress.ExpressApp.Utils;
//using DevExpress.Persistent.Base;

//namespace WinCTB_CTS.Module.Action
//{
//    public class CustomLayoutHelper
//    {
//        private static CustomLayoutHelper customLayoutHelper;
//        private Dictionary<string, HashSet<string>> classVisibleProperties = new Dictionary<string, HashSet<string>>();
//        private Dictionary<string, HashSet<string>> viewVisibleProperties = new Dictionary<string, HashSet<string>>();

//        private void RegisterProperties(string id, string visiblePropertiesString, Dictionary<string, HashSet<string>> visibleProperties)
//        {
//            string[] properties = visiblePropertiesString.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
//            if (!visibleProperties.ContainsKey(id))
//            {
//                visibleProperties.Add(id, new HashSet<string>());
//            }
//            foreach (string value in properties)
//            {
//                visibleProperties[id].Add(value);
//            }
//        }
//        public void RegisterClassVisibleProperties(string classId, string visibleProperties)
//        {
//            RegisterProperties(classId, visibleProperties, classVisibleProperties);
//        }
//        public void RegisterViewVisibleProperties(string viewId, string visibleProperties)
//        {
//            RegisterProperties(viewId, visibleProperties, viewVisibleProperties);
//        }
//        public HashSet<string> GetClassVisibleProperties(string classId)
//        {
//            HashSet<string> result;
//            classVisibleProperties.TryGetValue(classId, out result);
//            return result;
//        }
//        public HashSet<string> GetViewVisibleProperties(string viewId)
//        {
//            HashSet<string> result;
//            viewVisibleProperties.TryGetValue(viewId, out result);
//            return result;
//        }
//        public Boolean HasVisibleProperties(string classId)
//        {
//            return classVisibleProperties.ContainsKey(classId);
//        }
//        public static ModelDefaultAttribute FindModelDefaultAttribute(IMemberInfo memberInfo, string attributeName)
//        {
//            foreach (ModelDefaultAttribute modelDefaultAttribute in memberInfo.FindAttributes<ModelDefaultAttribute>())
//            {
//                if (modelDefaultAttribute.PropertyName == attributeName)
//                {
//                    return modelDefaultAttribute;
//                }
//            }
//            return null;
//        }
//        public static CustomLayoutHelper Instance
//        {
//            get
//            {
//                if (customLayoutHelper == null)
//                {
//                    customLayoutHelper = new CustomLayoutHelper();
//                }
//                return customLayoutHelper;
//            }
//        }
//    }

//    public class CustomModelViewsUpdater : ModelNodesGeneratorUpdater<ModelViewsNodesGenerator>
//    {
//        public override void UpdateNode(ModelNode node)
//        {
//            IModelViews views = (IModelViews)node;
//            foreach (IModelView modelView in views)
//            {
//                if ((modelView is IModelObjectView) && CustomLayoutHelper.Instance.HasVisibleProperties(((IModelObjectView)modelView).ModelClass.Name))
//                {
//                    modelView.Remove();
//                }
//                IModelDetailView modelDetailView = modelView as IModelDetailView;
//                if ((modelDetailView != null) && modelDetailView.ModelClass.TypeInfo.Implements<ICustomLayoutInfo>())
//                {
//                    GenerateNestedDetailViews(views, modelDetailView.ModelClass);
//                }
//            }
//        }
//        private void GenerateNestedDetailViews(IModelViews views, IModelClass modelClass)
//        {
//            string visibleProperties;
//            foreach (IModelMember modelMember in modelClass.AllMembers)
//            {
//                visibleProperties = ((IModelMemberExtender)modelMember).VisibleProperties;
//                if (visibleProperties != null)
//                {
//                    string nestedViewId = modelClass.TypeInfo.Name + "_" + modelMember.MemberInfo.Name + "_DetailView";
//                    IModelDetailView detailViewInfo = views.AddNode<IModelDetailView>(nestedViewId);
//                    detailViewInfo.ModelClass = views.Application.BOModel[modelMember.MemberInfo.MemberType.FullName];
//                    CustomLayoutHelper.Instance.RegisterViewVisibleProperties(detailViewInfo.Id, visibleProperties);
//                }
//            }
//        }
//    }

//    public class CustomBOModelMemberUpdater : ModelNodesGeneratorUpdater<ModelBOModelMemberNodesGenerator>
//    {
//        public override void UpdateNode(ModelNode node)
//        {
//            IModelClass modelClass = ((IModelClass)node.Parent);
//            if (CustomLayoutHelper.Instance.HasVisibleProperties(modelClass.Name))
//            {
//                GenerateModelMembers(modelClass, CustomLayoutHelper.Instance.GetClassVisibleProperties(modelClass.Name));
//            }
//        }
//        private void GenerateModelMembers(IModelClass modelClass, HashSet<string> visibleProperties)
//        {
//            foreach (string propertyName in visibleProperties)
//            {
//                if (modelClass.AllMembers[propertyName] == null)
//                {
//                    modelClass.OwnMembers.AddNode<IModelMember>(propertyName);
//                }
//            }
//        }
//    }

//    public class CustomBOModelUpdater : ModelNodesGeneratorUpdater<ModelBOModelClassNodesGenerator>
//    {
//        public override void UpdateNode(ModelNode node)
//        {
//            foreach (Type type in ((IModelSources)node.Application).BOModelTypes)
//            {
//                ITypeInfo typeinfo = XafTypesInfo.Instance.FindTypeInfo(type);
//                if (typeinfo.Implements<ICustomLayoutInfo>())
//                {
//                    GenerateModelClasses(node, typeinfo);
//                }
//            }
//        }
//        public override void UpdateCachedNode(ModelNode node)
//        {
//            foreach (Type type in ((IModelSources)node.Application).BOModelTypes)
//            {
//                ITypeInfo typeinfo = XafTypesInfo.Instance.FindTypeInfo(type);
//                if (typeinfo.Implements<ICustomLayoutInfo>())
//                {
//                    UpdateModelClasses(node, typeinfo);
//                }
//            }
//        }
//        private void UpdateModelClasses(ModelNode boModel, ITypeInfo typeTnfo)
//        {
//            foreach (IMemberInfo memberInfo in typeTnfo.OwnMembers)
//            {
//                IModelClass modelClass = ((IModelBOModel)boModel).GetClass(memberInfo.MemberType);
//                if (modelClass == null)
//                {
//                    Tracing.Tracer.LogWarning(String.Format("Cannot find the '{0}' node in the ModelApplication.BOModel collection. The '{1}{2}' file is outdated.", memberInfo.MemberType.FullName, ModelStoreBase.ModelCacheDefaultName, ModelStoreBase.ModelFileExtension));
//                }
//                else
//                {
//                    ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(memberInfo.MemberType);
//                    modelClass.SetValue<ITypeInfo>("TypeInfo", typeInfo);
//                }
//            }
//        }
//        private void GenerateModelClasses(ModelNode boModel, ITypeInfo typeTnfo)
//        {
//            foreach (IMemberInfo memberInfo in typeTnfo.OwnMembers)
//            {
//                ModelDefaultAttribute attribute = CustomLayoutHelper.FindModelDefaultAttribute(memberInfo, CustomDetailViewItemsGenarator.VisiblePropertiesAttribute);
//                if ((attribute != null) && (!string.IsNullOrEmpty(attribute.PropertyValue)))
//                {
//                    IModelClass modelClass = ((IModelBOModel)boModel)[memberInfo.MemberType.FullName];
//                    if (modelClass == null)
//                    {
//                        modelClass = boModel.AddNode<IModelClass>(memberInfo.MemberType.FullName);
//                        ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(memberInfo.MemberType);
//                        modelClass.SetValue<ITypeInfo>("TypeInfo", typeInfo);
//                    }
//                    CustomLayoutHelper.Instance.RegisterClassVisibleProperties(modelClass.Name, attribute.PropertyValue);
//                }
//            }
//        }
//    }

//    public class CustomDetailViewItemsGenarator : ModelNodesGeneratorUpdater<ModelDetailViewItemsNodesGenerator>
//    {
//        public const string GeneralId = "General";
//        public const string MainTabId = "MainTab";
//        public const string FooterId = "Footer";
//        public const string TabPageNameAttribute = "TabPageName";
//        public const string VisiblePropertiesAttribute = "VisibleProperties";
//        public const string ActionsContainerAttribute = "ActionsContainer";
//        public const string FooterAttribute = "Footer";
//        public const string TabPageIdAttribute = "TabPageId";

//        public override void UpdateNode(ModelNode node)
//        {
//            IModelDetailView detailView = node.Parent as IModelDetailView;
//            if (detailView != null)
//            {
//                if (detailView.ModelClass.TypeInfo.Implements<ICustomLayoutInfo>())
//                {
//                    GenerateCustomLayout(detailView);
//                }
//                if (CustomLayoutHelper.Instance.HasVisibleProperties(detailView.ModelClass.Name))
//                {
//                    GenerateNestedDetailViewLayout(detailView);
//                }
//            }
//        }
//        private void GenerateCustomLayout(IModelDetailView modelDetailView)
//        {
//            ITypeInfo iTypeInfo = modelDetailView.ModelClass.TypeInfo;
//            HashSet<string> actionContainerNames = new HashSet<string>();
//            foreach (IModelViewItem item in modelDetailView.Items)
//            {
//                IModelPropertyEditor editorInfo = (IModelPropertyEditor)item;
//                if (CustomLayoutHelper.FindModelDefaultAttribute(editorInfo.ModelMember.MemberInfo, VisiblePropertiesAttribute) != null)
//                {
//                    editorInfo.PropertyEditorType = typeof(DetailPropertyEditor);
//                }
//                string visiblePropertiesAttributeValue = ((IModelMemberExtender)item).VisibleProperties;
//                if (editorInfo.PropertyEditorType == typeof(DetailPropertyEditor) && !string.IsNullOrEmpty(visiblePropertiesAttributeValue))
//                {
//                    string nestedViewId = iTypeInfo.Name + "_" + editorInfo.PropertyName + "_DetailView";
//                    editorInfo.View = modelDetailView.Application.Views[nestedViewId];
//                }
//                ModelDefaultAttribute actionsContainerAttribute = CustomLayoutHelper.FindModelDefaultAttribute(editorInfo.ModelMember.MemberInfo, ActionsContainerAttribute);
//                if (actionsContainerAttribute != null)
//                {
//                    actionContainerNames.Add(actionsContainerAttribute.PropertyValue);
//                }
//            }
//            foreach (string name in actionContainerNames)
//            {
//                IModelActionContainerViewItem modelActionContainerViewItem = modelDetailView.Items.AddNode<IModelActionContainerViewItem>(name);
//            }
//        }
//        private void GenerateNestedDetailViewLayout(IModelDetailView modelDetailView)
//        {
//            List<IModelViewItem> items = new List<IModelViewItem>();
//            foreach (IModelViewItem item in modelDetailView.Items)
//            {
//                items.Add(item);
//            }
//            foreach (IModelViewItem itemR in items)
//            {
//                itemR.Remove();
//            }
//            int index = 0;
//            foreach (string propertyName in CustomLayoutHelper.Instance.GetViewVisibleProperties(modelDetailView.Id))
//            {
//                IModelPropertyEditor editor = modelDetailView.Items.AddNode<IModelPropertyEditor>(propertyName);
//                editor.ImmediatePostData = true;
//                editor.Index = index++;
//                editor.PropertyName = propertyName;
//                if (string.IsNullOrEmpty(((IModelViewItem)editor).Caption))
//                {
//                    ((IModelViewItem)editor).Caption = CaptionHelper.ConvertCompoundName(propertyName);
//                }
//            }
//        }
//    }
//    public interface IModelMemberExtender
//    {
//        string TabPageName { get; set; }
//        bool Footer { get; set; }
//        string VisibleProperties { get; set; }
//    }
//    public interface IModelLayoutGroupExtender
//    {
//        string TabPageId { get; set; }
//    }
//    [ModelInterfaceImplementor(typeof(IModelMemberExtender), "ModelMember")]
//    public interface IModelPropertyEditorClassMemberExtender : IModelMemberExtender { }
//}
