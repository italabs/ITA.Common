using System;

namespace ITA.Common.DeviceDetection
{
    public delegate void CardInsertEventHandler();
    public delegate void CardRemoveEventHandler();

    internal interface IDeviceDetector
    {
        event EventHandler OnInserted;
        event EventHandler OnRemoved;
        event EventHandler OnDeviceChanged;
    }

    public class DeviceDetector
    {
        private static DeviceDetector _instance = null;

        private IDeviceDetector m_Detector = null;
        private IDeviceDetector m_SmartDetector = null;
        
        public EventHandler OnDeviceChanged = null;
        public CardInsertEventHandler OnCardInserted = null;
        public CardRemoveEventHandler OnCardRemoved = null;

        public static DeviceDetector GetDeviceDetector()
        {
            if (_instance == null)
                _instance = new DeviceDetector();

            return _instance;
        }

        private DeviceDetector()
        {
            //m_Detector = new DefaultDetector();
            //m_Detector.OnDeviceChanged += new EventHandler(m_Detector_OnDeviceChanged);

            m_SmartDetector = SmartDetector.GetDetector();

            m_SmartDetector.OnInserted += new EventHandler(m_Detector_OnInserted);
            m_SmartDetector.OnRemoved += new EventHandler(m_Detector_OnRemoved);
        }

        private void m_Detector_OnRemoved(object sender, EventArgs e)
        {
            if (OnCardRemoved != null)
            {
                OnCardRemoved();
            }
        }

        private void m_Detector_OnInserted(object sender, EventArgs e)
        {
            if (OnCardInserted != null)
            {
                OnCardInserted();
            }
        }

        //private void m_Detector_OnDeviceChanged(object sender, EventArgs e)
        //{
        //    if (OnDeviceChanged != null)
        //    {
        //        OnDeviceChanged(null, EventArgs.Empty);
        //    }
        //}
    }
}
