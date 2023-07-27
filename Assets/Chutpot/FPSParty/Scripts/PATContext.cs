using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.context.impl;
using strange.extensions.command.api;
using strange.extensions.command.impl;
using strange.extensions.mediation.impl;
using strange.extensions.mediation.api;
using UnityEngine.SceneManagement;

namespace Chutpot.FPSParty
{
    public class PATContext : MVCSContext
    {
        public PATContext() : base() { }
        public PATContext(MonoBehaviour mono, bool autoStartup) : base(mono, autoStartup) { }

        protected override void addCoreComponents()
        {
            base.addCoreComponents();
            injectionBinder.Unbind<ICommandBinder>();
            injectionBinder.Bind<ICommandBinder>().To<SignalCommandBinder>().ToSingleton();
        }

        public override void AddView(object view)
        {

            //dont add base view because parent is included! Don't calling AddView because will cause recursing
            //TODO: this way of addView doesnt work with ChapterView, need to fix!
            SceneManager.MoveGameObjectToScene(((MonoBehaviour)view).transform.root.gameObject, (contextView as GameObject).scene);
            foreach (var childView in ((MonoBehaviour)view).GetComponentsInChildren<View>(true)) 
            {
                if (mediationBinder != null)
                {
                    mediationBinder.Trigger(MediationEvent.AWAKE, childView as IView);
                }
                else
                {
                    cacheView(childView as MonoBehaviour);
                }
            }
        }
        
    }
}
