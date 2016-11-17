ViveInputUtility for Unity - v1.4.7
Copyright 2016, HTC Corporation. All rights reserved.


Introduction:

- Vive Input Utility is a tool based on the SteamVR plugin that allows developers to access Vive device status in handy way.

- We also introduce a mouse pointer solution that works in 3D space and is compatible with the Unity Event System.

- By importing this utility, developers can save lots of time in writing redundant code to manage Vive devices.


Requirements:

- The SteamVR plugin must be installed. This can be found in Unity Asset Store.


Changes for v1.4.7:

* Now HandRole defines more then 2 controllers.

* Add some comment and description to public API.


Changes for v1.4.6:

* Fix a bug in the examples, now reticle posed correctly when scaling VROrigin.


Changes for v1.4.5:

* Fix a rare issue in Pointer3DInputModule when processing event raycast.


Changes for v1.4.4:

* Remove example 5 & 6 from package for release(still available in full package), since they are not good standard practices in VR for avoiding motion sickness by moving the player.

* Reset pointer's tranform(to align default laser pointer direction) in examples.

* Adjust default threshold to proper value in PoseStablizer & Pointer3DInputModule.

* Fix a bug in Pointer3DRaycaster that causes other input module to drive Pointer3DRaycaster(witch should be only driven by Poinster3DInputModule).

* Now Pointer3DRaycaster can optionally show event raycast line in editor for debugging.

* Add step by step tutorial document and example scene.

* Replace about document with developer guide.


Changes for v1.4.3:

* Update usage document(rewrite sample code).

* Add copyright terms.

* Define new controller button : FullTrigger(consider pressed only when trigger value is 1.0).

* Fix ViveInput.GetPadPressDelta and ViveInput.GetPadTouchDelta to work properly.

* Add scroll delta scale property for ViveRaycaster(to adjust scrolling sensitivity).

* Add PoseEaser effect settings and PoseEaserEditor to show properties.

* Add ViveInput.TriggerHapticPulse for triggering controller vibration.


Changes for v1.4.2:

* Update usage document.

* Reorder parameters in Pose.SetPose.

* Now click interval can be configured by setting ViveInput.clickInterval.


Changes for v1.4.1:

* Fix wrong initial status for ViveRole and ViveInput.

* Example: showLocalAvatar (property for LANGamePlayer) won't hide shadow (hide mesh only) if set to false.


Changes for v1.4.0:

* Separate PoseTracker module from VivePose.

* New tracking effect PoseFreezer.

* Reorganize folders.


Changes for v1.3.0:

* VivePose is now pure static class (Since Unity 5.3.5 fixed issue with double rendering of canvas on Vive VR, PoseUpdateMode is no longer needed).

* New components CanvasRaycastMethod and CanvasRaycastTarget.
  - CanvasRaycastMethod works like GraphicRaycastMethod, but use CanvasRaycastTarget component to target canvases, instead of asigning canvas property once at a time.


Changes for v1.2.0:

* Fix misspelling from ConvertRoleExtention to ConvertRoleExtension

* New containter class IndexedSet<T>

* New class ObjectPool<T> and relative class ListPool<T>, DictionaryPool<T>, IndexedSetPool<T>, to reduce allocating new containers.

* Change some data structure from LinkList to InedexedSet (VivePose, Pointer3DInputModule, BaseMultiMethodRaycaster, BaseVivePoseTracker).

* Rewrite GraphicRaycastMethod to align GraphicRaycaster's behaviour.


Changes for v1.1.0:

* New API VivePose.SetPose().

* New API VivePose.GetVelocity().

* New API VivePose.GetAngularVelocity().

* Fix some null reference in VivePose.