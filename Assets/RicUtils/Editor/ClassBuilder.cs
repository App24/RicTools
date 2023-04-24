using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RicUtils.Editor
{
    public sealed class ClassBuilder
    {
        private string namespaceName;

        private int indentation;
        private ClassData classData;
        private List<string> usings = new List<string>();

        public ClassBuilder SetNamespace(string namespaceName)
        {
            this.namespaceName = namespaceName;
            return this;
        }

        /*public ClassBuilder AddIndentation(int amount = 1)
        {
            indentation += amount;
            return this;
        }

        public ClassBuilder RemoveIndentation(int amount = 1)
        {
            indentation -= amount;
            return this;
        }*/

        public ClassBuilder SetClassName(AccessModifier accessModifier, Keyword keyword, string name, string baseClass = "")
        {
            classData = new ClassData(accessModifier, keyword, name, ClassType.Class, baseClass);
            return this;
        }

        public ClassBuilder AddUsing(string usingName)
        {
            usings.Add(usingName);
            return this;
        }

        public string Build()
        {
            string toReturn = "";

            foreach (var @using in usings)
            {
                toReturn += $"using {@using}";
                AddEndLine(ref toReturn);
                AddNewLine(ref toReturn);
            }

            AddNewLine(ref toReturn);

            if (!string.IsNullOrEmpty(namespaceName))
            {
                toReturn += $"namespace {namespaceName}";
                AddNewLine(ref toReturn);
                AddOpenCurlyBrackets(ref toReturn);
                indentation++;
            }

            {
                toReturn += AddIndentation(classData.accessModifier.ToString().ToLower() + " ");
                if (classData.keyword != Keyword.None)
                {
                    toReturn += classData.keyword.ToString().ToLower() + " ";
                }
                toReturn += classData.type.ToString().ToLower() + " ";
                toReturn += classData.name;
                if (!string.IsNullOrEmpty(classData.baseClass))
                {
                    toReturn += " : " + classData.baseClass;
                }
                AddNewLine(ref toReturn);
                AddOpenCurlyBrackets(ref toReturn);
                indentation++;
            }

            while (indentation > 0)
            {
                indentation--;
                AddCloseCurlyBrackets(ref toReturn);
            }

            return toReturn;
        }

        private string AddIndentation(string toAdd)
        {
            var text = "";
            for (int i = 0; i < indentation; i++)
            {
                text += "\t";
            }
            return text + toAdd;
        }

        private void AddNewLine(ref string text)
        {
            text += Environment.NewLine;
        }

        private void AddEndLine(ref string text)
        {
            text += ";";
        }

        private void AddOpenCurlyBrackets(ref string text, bool addNewLine = true)
        {
            text += AddIndentation("{");
            if (addNewLine)
            {
                AddNewLine(ref text);
            }
        }

        private void AddCloseCurlyBrackets(ref string text, bool addNewLine = true)
        {
            text += AddIndentation("}");
            if (addNewLine)
            {
                AddNewLine(ref text);
            }
        }
    }

    public enum AccessModifier
    {
        Public,
        Private,
        Protected,
        Internal,
    }

    public enum Keyword
    {
        None,
        Static,
        Abstract
    }

    internal enum ClassType
    {
        Class,
        Enum,
        Struct,
        Interface,
    }

    internal struct ClassData
    {
        public AccessModifier accessModifier;
        public Keyword keyword;
        public string name;
        public ClassType type;
        public string baseClass;

        public ClassData(AccessModifier accessModifier, Keyword keyword, string name, ClassType type, string baseClass)
        {
            this.accessModifier = accessModifier;
            this.keyword = keyword;
            this.name = name;
            this.type = type;
            this.baseClass = baseClass;
        }
    }
}
