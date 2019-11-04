using System;
using System.Data.SqlClient;
using System.Xml.Serialization;
using USC.GISResearchLab.Common.Addresses;
using USC.GISResearchLab.Common.Geometries;
using USC.GISResearchLab.Geocoding.Core.Algorithms.FeatureMatchingMethods;
using USC.GISResearchLab.Geocoding.Core.Queries.Parameters;

namespace USC.GISResearchLab.Geocoding.Core.ReferenceDatasets.ReferenceSourceQueries
{
    [Serializable]
    public class ReferenceSourceQuery
    {

        #region Properties

        public string SourceName { get; set; }
        public string MethodName { get; set; }
        public string Side { get; set; }
        public RelaxableStreetAddress RelaxableStreetAddress { get; set; }
        public StreetAddress StreetAddress { get; set; }
        public StreetAddress AttemptedStreetAddress { get; set; }
        public string Error { get; set; }
        public int NumberOfResultRows { get; set; }
        public PlaceTypes PlaceType { get; set; }

        [XmlIgnore]
        public SqlCommand SqlCommand { get; set; }

        public string Sql
        {
            get
            {
                string ret = "";
                if (SqlCommand != null)
                {
                    ret = SqlCommand.CommandText;
                }
                return ret;
            }
        }

        public string SqlQueryable
        {
            get
            {
                string ret = "";
                if (SqlCommand != null)
                {

                    if (SqlCommand.Parameters != null)
                    {
                        foreach (SqlParameter parameter in SqlCommand.Parameters)
                        {
                            ret = "DECLARE @" + parameter.ParameterName + parameter.SqlValue + " ;";
                        }
                    }

                    ret = SqlCommand.CommandText;
                }
                return ret;
            }
        }

        public TimeSpan Duration
        {
            get
            {
                TimeSpan ret = new TimeSpan(99, 00, 00);
                if (TimeStart != null && TimeEnd != null)
                {
                    ret = new TimeSpan(TimeEnd.Ticks - TimeStart.Ticks);
                }
                return ret;
            }
        }

        public bool Attempted { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public string MatchTypesString { get; set; }
        public string[] RelaxedAttributes { get; set; }

        public FeatureMatchTypes FeatureMatchType { get; set; }

        public string RelaxedAttributesString
        {
            get
            {
                string ret = "";
                if (RelaxedAttributes != null)
                {
                    for (int i = 0; i < RelaxedAttributes.Length; i++)
                    {
                        if (i > 0)
                        {
                            ret += ",";
                        }
                        ret += RelaxedAttributes[i];
                    }
                }
                return ret;
            }
        }

        public Geometry[] ResultGeometries { get; set; }

        #endregion

        //public ReferenceSourceQuery(ParameterSet parameterSet, string sql, FeatureMatchTypes matchType)
        //    :this(parameterSet, sql, matchType, PlaceTypes.unknown)
        //{
        //}

        public ReferenceSourceQuery()
            : this(null, null, FeatureMatchTypes.Unknown, PlaceTypes.unknown)
        {
        }

        public ReferenceSourceQuery(ParameterSet parameterSet, SqlCommand sqlCommand, FeatureMatchTypes matchType, PlaceTypes placeType)
        {
            if (parameterSet != null)
            {
                SourceName = parameterSet.SourceStr;
                MethodName = parameterSet.MethodStr;
                StreetAddress = parameterSet.StreetAddress;
                RelaxableStreetAddress = parameterSet.RelaxableStreetAddress;
                AttemptedStreetAddress = parameterSet.AttemptedStreetAddress;
            }

            SqlCommand = sqlCommand;
            FeatureMatchType = matchType;
            PlaceType = placeType;

            if (RelaxableStreetAddress != null)
            {
                if (RelaxableStreetAddress.RelaxedAttributes != null)
                {
                    RelaxedAttributes = new string[RelaxableStreetAddress.RelaxedAttributes.Count];
                    for (int i = 0; i < RelaxableStreetAddress.RelaxedAttributes.Count; i++)
                    {
                        AddressComponents attribute = (AddressComponents)RelaxableStreetAddress.RelaxedAttributes[i];
                        RelaxedAttributes[i] = AddressComponentManager.GetAddressComponentName(attribute);
                    }
                }
            }
            MatchTypesString = FeatureMatcher.GetFeatureMatchTypeName(FeatureMatchType);
        }

        public void Start()
        {
            TimeStart = DateTime.Now;
        }

        public void End()
        {
            TimeEnd = DateTime.Now;
        }

        public void End(int recordCount)
        {
            TimeEnd = DateTime.Now;
            NumberOfResultRows = recordCount;
        }

        public void AddResultGeometry(Geometry geometry)
        {
            if (ResultGeometries == null)
            {
                ResultGeometries = new Geometry[1];
                ResultGeometries[0] = geometry;
            }
            else
            {
                Geometry[] temp = new Geometry[ResultGeometries.Length + 1];
                ResultGeometries.CopyTo(temp, 0);
                temp[temp.Length - 1] = geometry;
                ResultGeometries = temp;
            }
        }

        public override string ToString()
        {
            string ret = "";
            ret += Attempted;
            ret += "\t";
            ret += Duration;
            ret += "\t";
            ret += NumberOfResultRows;
            ret += "\t";
            ret += RelaxedAttributes;
            ret += "\t";
            ret += Sql;
            ret += "\t";
            ret += StreetAddress;
            return ret;
        }
    }
}
