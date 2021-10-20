using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ManipulatorMod.Modules.Components
{
    public class SetChildRotation : MonoBehaviour
    {
        public void SetRotation(string childName, Quaternion targetRotation)
        {
            Transform target = this.gameObject.transform.Find(childName);
            target.rotation = targetRotation;
        }
    }
}
