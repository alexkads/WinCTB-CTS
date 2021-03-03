//using DevExpress.ExpressApp.DC;
//using DevExpress.ExpressApp.Editors;
//using DevExpress.ExpressApp.Layout;
//using DevExpress.ExpressApp.Model;
//using DevExpress.ExpressApp.Model.Core;
//using DevExpress.ExpressApp.Model.NodeGenerators;
//using DevExpress.ExpressApp.Utils;

//namespace WinCTB_CTS.Module.Action
//{
//    public class CustomDetailViewLayoutGenarator : ModelNodesGeneratorUpdater<ModelDetailViewLayoutNodesGenerator>
//    {

//        public override void UpdateNode(ModelNode node)
//        {
//            if (node.Parent is IModelDetailView && ((IModelDetailView)(node.Parent)).ModelClass.TypeInfo.Implements<ICustomLayoutInfo>())
//            {
//                IModelDetailView detailViewInfo = ((IModelDetailView)node.Parent);
//                GenerateTabbedLayout(detailViewInfo);
//            }
//        }
//        private void GenerateTabbedLayout(IModelDetailView detailViewInfo)
//        {
//            if (detailViewInfo.Layout["Main"] != null)
//            {
//                detailViewInfo.Layout["Main"].Remove();
//            }
//            IModelLayoutGroup main = detailViewInfo.Layout.AddNode<IModelLayoutGroup>(ModelDetailViewLayoutNodesGenerator.MainLayoutGroupName);
//            IModelLayoutGroup generalNode = null;
//            IModelTabbedGroup tabNode = null;
//            IModelLayoutGroup footerNode = null;
//            foreach (IModelViewItem modelViewItem in detailViewInfo.Items)
//            {
//                IModelPropertyEditor editor = modelViewItem as IModelPropertyEditor;
//                if (editor != null)
//                {
//                    if (FindModelDefaultAttribute(editor.ModelMember.MemberInfo, CustomDetailViewItemsGenarator.VisiblePropertiesAttribute) != null)
//                    {
//                        editor.PropertyEditorType = typeof(DetailPropertyEditor);// editor.ModelMember.EditorsInfo[EditorAliases.DetailPropertyEditor].DefaultEditor;
//                    }
//                    editor.ImmediatePostData = true;
//                    IModelMemberExtender item = (IModelMemberExtender)editor;
//                    string tabPageName = item.TabPageName;
//                    if (string.IsNullOrEmpty(tabPageName))
//                    {
//                        if (item.Footer)
//                        {
//                            if (footerNode == null)
//                            {
//                                footerNode = main.AddNode<IModelLayoutGroup>(CustomDetailViewItemsGenarator.FooterId);
//                                footerNode.Index = 5;
//                            }
//                            AddLayoutItemNode(footerNode, editor);
//                        }
//                        else
//                        {
//                            if (generalNode == null)
//                            {
//                                generalNode = main.AddNode<IModelLayoutGroup>(CustomDetailViewItemsGenarator.GeneralId);
//                                generalNode.Index = editor.Index.HasValue ? editor.Index : 0;
//                            }
//                            AddLayoutItemNode(generalNode, editor);
//                        }
//                    }
//                    else
//                    {
//                        if (tabNode == null)
//                        {
//                            tabNode = main.AddNode<IModelTabbedGroup>(CustomDetailViewItemsGenarator.MainTabId);
//                            tabNode.Index = editor.Index.HasValue ? editor.Index : 1;
//                        }
//                        IModelLayoutGroup group = AddEditorToTabbedGroup(tabNode, tabPageName, editor);
//                        if (editor.ModelMember.Index >= 0)
//                        {
//                            group.Index = editor.ModelMember.Index;
//                        }
//                    }
//                }
//            }
//        }
//        private static ModelDefaultAttribute FindModelDefaultAttribute(IMemberInfo memberInfo, string attributeName)
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
//        private void AddLayoutItemNode(IModelLayoutGroup layoutGroup, IModelPropertyEditor editor)
//        {
//            ModelDefaultAttribute modelDefaultAttribute = FindModelDefaultAttribute(editor.ModelMember.MemberInfo, CustomDetailViewItemsGenarator.ActionsContainerAttribute);
//            if (modelDefaultAttribute != null)
//            {
//                IModelViewItem container = editor.Parent.GetNode(modelDefaultAttribute.PropertyValue) as IModelViewItem;
//                if ((container != null))
//                {
//                    IModelLayoutViewItem item = layoutGroup.AddNode<IModelLayoutViewItem>(container.Id);
//                    item.ViewItem = container;
//                    item.Index = editor.Index;
//                }
//            }
//            else
//            {
//                IModelLayoutViewItem item = layoutGroup.AddNode<IModelLayoutViewItem>(editor.PropertyName);
//                item.ViewItem = editor;
//                item.Index = editor.Index;
//            }
//        }
//        private IModelLayoutGroup AddEditorToTabbedGroup(IModelTabbedGroup rootTabNode, string tabId, IModelPropertyEditor editor)
//        {
//            return AddEditorToTabbedGroup(rootTabNode, tabId, editor, FlowDirection.Horizontal);
//        }
//        private IModelLayoutGroup AddEditorToTabbedGroup(IModelTabbedGroup rootTabNode, string tabId, IModelPropertyEditor editor, FlowDirection direction)
//        {
//            IModelLayoutGroup rootTabPageNode = (IModelLayoutGroup)rootTabNode[tabId];
//            if (rootTabPageNode == null)
//            {
//                rootTabPageNode = rootTabNode.AddNode<IModelLayoutGroup>(tabId);
//                rootTabPageNode.Caption = CaptionHelper.ConvertCompoundName(tabId);
//                rootTabPageNode.Direction = direction;
//            }
//            AddLayoutItemNode(rootTabPageNode, editor);
//            return rootTabPageNode;
//        }
//    }
//}
