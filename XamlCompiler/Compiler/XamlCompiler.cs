using System;
using System.IO;
using Mono.Cecil;
using Portable.Xaml;
using Stateless;

namespace XamlCompiler.Compiler
{
    internal class XamlCompiler
    {
        private readonly AssemblyDefinition _assembly;
        private readonly StateMachine<State, XamlParseTrigger> _stateMachine;

        private readonly StateMachine<State, XamlParseTrigger>.TriggerWithParameters<XamlType> _objectStart;
        private readonly StateMachine<State, XamlParseTrigger>.TriggerWithParameters<object> _attributeValue;

        private ObjectInfo _currentObjectInfo;

        public XamlCompiler(string assemblyName, Version assemblyVersion)
        {
            _assembly = AssemblyDefinition.CreateAssembly(
                new AssemblyNameDefinition(assemblyName, assemblyVersion),
                assemblyName,
                ModuleKind.Dll);

            _stateMachine = new StateMachine<State, XamlParseTrigger>(State.Init);
            _objectStart = _stateMachine.SetTriggerParameters<XamlType>(XamlParseTrigger.ObjectStart);
            _attributeValue = _stateMachine.SetTriggerParameters<object>(XamlParseTrigger.AttributeValue);

            InitializeStateMachine();
        }

        private void InitializeStateMachine()
        {
            _stateMachine.Configure(State.Init)
                .Permit(XamlParseTrigger.ObjectStart, State.InsideObject);

            _stateMachine.Configure(State.End)
                .OnEntry(GenerateType);

            _stateMachine.Configure(State.InsideObject)
                .OnEntryFrom(_objectStart, InitializeObject)
                .OnEntryFrom(_attributeValue, SetAttributeValue)
                .Ignore(XamlParseTrigger.AttributeValue)
                .Permit(XamlParseTrigger.ClassAttribute, State.InsideClassAttribute)
                .Permit(XamlParseTrigger.ObjectEnd, State.End);

            _stateMachine.Configure(State.InsideClassAttribute)
                .Permit(XamlParseTrigger.AttributeValue, State.InsideObject);
        }

        public void WriteAssembly(Stream output) => _assembly.Write(output);

        public void OnStartObject(XamlType type) => _stateMachine.Fire(_objectStart, type);
        public void OnClassAttribute() => _stateMachine.Fire(XamlParseTrigger.ClassAttribute);
        public void OnAttributeValue(object value) => _stateMachine.Fire(_attributeValue, value);
        public void OnEndObject() => _stateMachine.Fire(XamlParseTrigger.ObjectEnd);

        private void InitializeObject(XamlType type) => _currentObjectInfo = new ObjectInfo
        {
            BaseType = type
        };

        private void SetAttributeValue(object value, StateMachine<State, XamlParseTrigger>.Transition transition)
        {
            switch (transition.Source)
            {
                case State.InsideClassAttribute:
                    _currentObjectInfo.TypeName = (string) value;
                    break;
                default:
                    throw new NotSupportedException(
                        $"Invalid transition from {transition.Source} through {nameof(SetAttributeValue)}");
            }
        }

        private void GenerateType() => _currentObjectInfo.AddTypeDefinition(_assembly.MainModule);
    }
}
