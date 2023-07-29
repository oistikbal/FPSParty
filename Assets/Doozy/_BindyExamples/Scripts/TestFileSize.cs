using UnityEngine;

namespace Doozy.Sandbox.Bindy
{
    public class TestFileSize : MonoBehaviour
    {
        public float FloatValue;
        
        public long longValue
        {
            get => (long)FloatValue;
            set => FloatValue = value;
        }
    }
}
