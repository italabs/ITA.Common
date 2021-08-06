using System;
using System.Xml.Serialization;

namespace ITA.Wizards.UpdateWizard.Model
{
    /// <summary>
    /// Container for server upgrade rules
    /// </summary>
    [XmlRoot("upgrade")]
    public class UpdateRule
    {
        public UpdateRule()
        {
        
        }

        [XmlAttribute("minimal")]
        public string __minimal;

        private Version _minimal;

        [XmlIgnore()]
        public Version Minimal
        {
            get
            {
                if (this._minimal == null && !string.IsNullOrEmpty(this.__minimal))
                {
                    this._minimal = new Version(this.__minimal);
                }

                return this._minimal;
            }
        }

        [XmlArray("steps")]
        [XmlArrayItem("step", Type = typeof(DatabaseUpdateStep))]
        public DatabaseUpdateStep[] Steps = null;
    }

    /// <summary>
    /// Update step type
    /// </summary>
    public enum DatabaseUpdateStepType
    {
        /// <summary>
        /// execute SQL script
        /// </summary>
        [XmlEnum("sql")]
        Sql,

        /// <summary>
        /// execute custom .net managed code
        /// </summary>
        [XmlEnum("net")]
        Net,

        /// <summary>
        /// No update action
        /// </summary>
        [XmlEnum("none")]
        None
    }

    /// <summary>
    /// Single step parameters
    /// </summary>
    [Serializable()]
    public class UpdateStepArgument
    {
        public UpdateStepArgument()
        { 
        
        }

        [XmlAttribute("name")]
        public string Name;

        [XmlAttribute("value")]
        public string Value;

        [XmlAttribute("description")]
        public string Description;
    }

    /// <summary>
    /// Single upgrade step
    /// </summary>
    [Serializable()]
    public class DatabaseUpdateStep
    {

        public DatabaseUpdateStep()
        {

        }

        [XmlAttribute("id")]
        public int Id;

        [XmlAttribute("from")]
        public string __from;

        private Version _from;

        [XmlIgnore()]
        public Version From
        {
            get
            {
                if (this._from == null && !string.IsNullOrEmpty(this.__from))
                {
                    this._from = new Version(this.__from);
                }

                return this._from;
            }
        }

        [XmlAttribute("to")]
        public string __to;

        private Version _to;

        [XmlIgnore()]
        public Version To
        {
            get
            {
                if (this._to == null && !string.IsNullOrEmpty(this.__to))
                {
                    this._to = new Version(this.__to);
                }

                return this._to;
            }
        }

        [XmlAttribute("type")]
        public DatabaseUpdateStepType Type;

        [XmlAttribute("source")]
        public string Source;

        [XmlAttribute("description")]
        public string Description;

        [XmlAttribute("complexity")]
        public int Complexity = 1;

        [XmlAttribute("timeout")]
        public int Timeout = 0;

        [XmlArray("args")]
        [XmlArrayItem(ElementName = "arg", Type = typeof(UpdateStepArgument))]
        public UpdateStepArgument[] Arguments = null;
    }     
}
