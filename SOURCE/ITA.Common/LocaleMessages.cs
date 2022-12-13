using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;
using log4net;

namespace ITA.Common
{
    public sealed class ResourceMappingInfo : IEquatable<ResourceMappingInfo>
    {
        public Assembly ResourceAssembly;
        public string ResourceName;

        public bool Equals(ResourceMappingInfo o)
        {
            if (o == null)
                return false;

            return this.Equals((object) o);
        }

        public override bool Equals(object o)
        {
            if (o == null)
                return false;

            if (!(o is ResourceMappingInfo))
                return false;

            ResourceMappingInfo rmi = o as ResourceMappingInfo;

            return rmi.ResourceName == this.ResourceName && rmi.ResourceAssembly == this.ResourceAssembly;
        }

        public override int GetHashCode()
        {
            int asmHash = 0;

            if (ResourceAssembly != null)
                asmHash = ResourceAssembly.GetHashCode();

            int nameHash = 0;
            if (ResourceName != null)
                nameHash = ResourceName.GetHashCode();

            return asmHash ^ nameHash;
        }

        public override string ToString()
        {
            return String.Format("ResourceMappingInfo{{ResourceAssembly={0}, ResourceName={1}}}", this.ResourceAssembly, this.ResourceName);
        }
    }

    /// <summary>
    /// LocaleMessages class. It's a new version of <see cref="ErrorMessages"/> class that is considered obsolete now.
    /// Supports 1-to-many type-resource relationships and contains public static instance of itself.
    /// </summary>
    /// <remarks>Name is grammatically incorrect, but it is much shorter that LocalizationMessages.</remarks>
    public class LocaleMessages
    {
        private static readonly ILog logger = Log4NetItaHelper.GetLogger(typeof(LocaleMessages).Name);
        private readonly ConcurrentDictionary<string,List<ResourceMappingInfo>> type2ResourceTable;
        private static LocaleMessages globalInstance = new LocaleMessages();

        public LocaleMessages()
        {
            type2ResourceTable = new ConcurrentDictionary<string, List<ResourceMappingInfo>>();
        }

        public static LocaleMessages GlobalInstance
        {
            get { return globalInstance; }
        }
        
        public void RegisterAssembly(Assembly A)
        {
            logger.Debug("RegisterAssembly() begin.");
            try
            {
                object[] attr = A.GetCustomAttributes(typeof (AssemblyResourceInfoAttribute), false);

                foreach (AssemblyResourceInfoAttribute attrib in attr)
                {
                    if (attrib.ExceptionType == null || String.IsNullOrEmpty(attrib.ResourceName))
                    {
                        logger.WarnFormat("Malformed AssemblyResourceInfoAttribute found inside assembly '{0}'. Its ExceptionType or ResourceName is null", A.FullName);
                        continue;
                    }

                    var rmi = new ResourceMappingInfo
                    {
                        ResourceAssembly = A,
                        ResourceName = attrib.ResourceName
                    };

                    logger.Debug(rmi);

                    type2ResourceTable.AddOrUpdate(attrib.ExceptionType.FullName, key =>
                    {
                        logger.DebugFormat("type2ResourceTable doesn't contain type '{0}'", key);
                        return new List<ResourceMappingInfo>
                        {
                            rmi
                        };
                    }, (_, list) =>
                    {
                        if (!list.Contains(rmi))
                        {
                            list.Add(rmi);
                        }

                        return list;
                    });
                }
            }
            finally
            {
                logger.Debug("RegisterAssembly() end.");
            }
        }

        public string GetString(string typeFullName, string resourceID, CultureInfo ci, params object[] args)
        {
            string strMessage = null;
            logger.Debug("GetString() begin.");

            logger.DebugFormat("typeFullName:{0}", typeFullName);
            logger.DebugFormat("resourceID:{0}", resourceID);
            logger.DebugFormat("ci:{0}", ci);

            if (args != null)
            {
                logger.Debug("args:");
                foreach (object o in args)
                {
                    logger.DebugFormat("arg:{0}", o);
                }
            }

            try
            {
                if (!type2ResourceTable.TryGetValue(typeFullName, out List<ResourceMappingInfo> rmiList))
                {
                    return null;
                }

                logger.DebugFormat("type2ResourceTable contains mapping list for type '{0}' with {1} values.", typeFullName, rmiList.Count);

                foreach (ResourceMappingInfo info in rmiList)
                {
                    logger.DebugFormat("ExceptionInfo.ResourceName:{0}", info.ResourceName);
                    logger.DebugFormat("ExceptionInfo.ResourceAssembly:{0}", info.ResourceAssembly);

                    var rm = new ResourceManager(info.ResourceName, info.ResourceAssembly);

                    try
                    {
                        strMessage = rm.GetString(resourceID, ci);
                        logger.DebugFormat("ResourceManager.GetString returned: {0}", strMessage);
                    }
                    catch (MissingManifestResourceException mre)
                    {
                        // no resorces found;
                        logger.Info(mre);
                    }
                    catch (InvalidOperationException ioe)
                    {
                        // is not a string
                        logger.Error(ioe);
                    }

                    if (strMessage != null)
                        break;
                }

                if (strMessage != null && args != null && args.Length > 0)
                {
                    try
                    {
                        logger.DebugFormat("formatting strMessage:{0}", strMessage);
                        // trying to format message
                        strMessage = string.Format(strMessage, args);
                        logger.DebugFormat("formatted strMessage:{0}", strMessage);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                        // unable to format message
                        strMessage = string.Format("{0} (unable to format message)", strMessage);
                    }
                }

                return strMessage;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return strMessage;
            }
            finally
            {
                logger.Debug("GetString() end.");
            }
        }

        public string GetString(Type objType, int resourceID, CultureInfo ci, params object[] args)
        {
            return GetString(objType.FullName, resourceID.ToString(), ci, args);
        }

        public string GetString(Type objType, string resourceID, params object[] Args)
        {
            return GetString(objType.FullName, resourceID, Thread.CurrentThread.CurrentUICulture, Args);
        }

        public string GetString(Type objType, string resourceID, CultureInfo ci, params object[] Args)
        {
            return GetString(objType.FullName, resourceID, ci, Args);
        }

        public string GetString(Type objType, int resourceID, params object[] Args)
        {
            return GetString(objType.FullName, resourceID.ToString(), Thread.CurrentThread.CurrentUICulture, Args);
        }

        public string GetString(string typeFullName, int resourceID, CultureInfo ci, params object[] args)
        {
            return GetString(typeFullName, resourceID.ToString(), ci, args);
        }

        public string GetString(string typeFullName, string resourceID, params object[] Args)
        {
            return GetString(typeFullName, resourceID, Thread.CurrentThread.CurrentUICulture, Args);
        }

        public string GetString(string typeFullName, int resourceID, params object[] Args)
        {
            return GetString(typeFullName, resourceID.ToString(), Thread.CurrentThread.CurrentUICulture, Args);
        }
    }
}