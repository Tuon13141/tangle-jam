using System.Collections;
using System.Collections.Generic;
using Sonat;
using UnityEngine;

public class TestGameManager : KernelLoadedView
{
   protected override void OnKernelLoaded()
   {
      base.OnKernelLoaded();
      Kernel.Resolve<AdsManager>().CheckShowBanner();
      Kernel.Resolve<FireBaseController>().LogEvent("start_game");
   }
}
