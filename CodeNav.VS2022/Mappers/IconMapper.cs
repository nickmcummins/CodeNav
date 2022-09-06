using CodeNav.Models;
using Microsoft.VisualStudio.Imaging.Interop;
using static CodeNav.Models.CodeItemAccessEnum;
using static Microsoft.VisualStudio.Imaging.KnownMonikers;

namespace CodeNav.Mappers
{
    public static class IconMapper
    {
        public static ImageMoniker MapMoniker(CodeItemKindEnum kind, CodeItemAccessEnum access)
        {
            switch (kind)
            {
                case CodeItemKindEnum.Namespace:
                    if (access == Public) return NamespacePublic;
                    else if (access == Private) return NamespacePrivate;
                    else if (access == Protected) return NamespaceProtected;
                    else if (access == CodeItemAccessEnum.Internal) return NamespaceInternal;
                    else if (access == Sealed) return NamespaceSealed;
                    else return Namespace;
                case CodeItemKindEnum.Class:
                    if (access == Public) return ClassPublic;
                    else if (access == Private) return ClassPrivate;
                    else if (access == Protected) return ClassProtected;
                    else if (access == CodeItemAccessEnum.Internal) return ClassInternal;
                    else if (access == Sealed) return ClassSealed;
                    else return Class;
                case CodeItemKindEnum.Constant:
                    if (access == Public) return ConstantPublic;
                    else if (access == Private) return ConstantPrivate;
                    else if (access == Protected) return ConstantProtected;
                    else if (access == CodeItemAccessEnum.Internal) return ConstantInternal;
                    else if (access == Sealed) return ConstantSealed;
                    else return Constant;
                case CodeItemKindEnum.Delegate:
                    if (access == Public) return DelegatePublic;
                    else if (access == Private) return DelegatePrivate;
                    else if (access == Protected) return DelegateProtected;
                    else if (access == CodeItemAccessEnum.Internal) return DelegateInternal;
                    else if (access == Sealed) return DelegateSealed;
                    else return Delegate; 
                case CodeItemKindEnum.Enum:
                    if (access == Public) return EnumerationPublic;
                    else if (access == Private) return EnumerationPrivate;
                    else if (access == Protected) return EnumerationProtected;
                    else if (access == CodeItemAccessEnum.Internal) return EnumerationInternal;
                    else if (access == Sealed) return EnumerationSealed;
                    else return Enumeration;
                case CodeItemKindEnum.EnumMember:
                    if (access == Public) return EnumerationItemPublic;
                    else if (access == Private) return EnumerationItemPrivate;
                    else if (access == Protected) return EnumerationItemProtected;
                    else if (access == CodeItemAccessEnum.Internal) return EnumerationItemInternal;
                    else if (access == Sealed) return EnumerationItemSealed;
                    else return EnumerationItemPublic;
                case CodeItemKindEnum.Event:
                    if (access == Public) return EventPublic;
                    else if (access == Private) return EventPrivate;
                    else if (access == Protected) return EventProtected;
                    else if (access == CodeItemAccessEnum.Internal) return EventInternal;
                    else if (access == Sealed) return EventSealed;
                    else return Event;
                case CodeItemKindEnum.Interface:
                    if (access == Public) return InterfacePublic;
                    else if (access == Private) return InterfacePrivate;
                    else if (access == Protected) return InterfaceProtected;
                    else if (access == CodeItemAccessEnum.Internal) return InterfaceInternal;
                    else if (access == Sealed) return InterfaceSealed;
                    else return Interface;
                case CodeItemKindEnum.Constructor:
                case CodeItemKindEnum.Method:
                    if (access == Public) return MethodPublic;
                    else if (access == Private) return MethodPrivate;
                    else if (access == Protected) return MethodProtected;
                    else if (access == CodeItemAccessEnum.Internal) return MethodInternal;
                    else if (access == Sealed) return MethodSealed;
                    else return Method;
                case CodeItemKindEnum.Property:
                case CodeItemKindEnum.Indexer:
                    if (access == Public) return PropertyPublic;
                    else if (access == Private) return PropertyPrivate;
                    else if (access == Protected) return PropertyProtected;
                    else if (access == CodeItemAccessEnum.Internal) return PropertyInternal;
                    else if (access == Sealed) return PropertySealed;
                    else return Property;
                case CodeItemKindEnum.Struct:
                case CodeItemKindEnum.Record:
                    if (access == Public) return StructurePublic;
                    else if (access == Private) return StructurePrivate;
                    else if (access == Protected) return StructureProtected;
                    else if (access == CodeItemAccessEnum.Internal) return StructureInternal;
                    else if (access == Sealed) return StructureSealed;
                    else return Structure;
                case CodeItemKindEnum.Variable:
                    if (access == Public) return FieldPublic;
                    else if (access == Private) return FieldPrivate;
                    else if (access == Protected) return FieldProtected;
                    else if (access == CodeItemAccessEnum.Internal) return FieldInternal;
                    else if (access == Sealed) return FieldSealed;
                    else return Field;
                case CodeItemKindEnum.Switch:
                    return FlowSwitch;
                case CodeItemKindEnum.SwitchSection:
                    return FlowDecision;
                case CodeItemKindEnum.StyleRule:
                    return Rule;
                case CodeItemKindEnum.PageRule:
                    return PageStyle;
                case CodeItemKindEnum.NamespaceRule:
                    return Namespace;
                case CodeItemKindEnum.MediaRule:
                    return Media;
                case CodeItemKindEnum.FontFaceRule:
                    return Font;
                default:
                    if (access == Public) return PropertyPublic;
                    else if (access == Private) return PropertyPrivate;
                    else if (access == Protected) return PropertyProtected;
                    else if (access == CodeItemAccessEnum.Internal) return PropertyInternal;
                    else if (access == Sealed) return PropertySealed;
                    else return Property;
            }
        }
    }
}
