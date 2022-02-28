/************************************************************************************

Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.  

See SampleFramework license.txt for license terms.  Unless required by applicable law 
or agreed to in writing, the sample code is provided “AS IS” WITHOUT WARRANTIES OR 
CONDITIONS OF ANY KIND, either express or implied.  See the license for specific 
language governing permissions and limitations under the license.

************************************************************************************/

using UnityEngine;
using UnityEngine.Assertions;

namespace OculusSampleFramework
{
	public class CustomControllerBoxController : MonoBehaviour
	{
		[SerializeField] private TrainLocomotive _locomotive = null;
		[SerializeField] private CowController _cowController = null;
		public HandleMovement HandleMovementObject;
		public GameObject RightHandOculus;
		public GameObject LeftHandLeap;
		private bool isFixed = false;
		public HandleCube CollisionHandlerObject;

		private void Awake()
		{
			Assert.IsNotNull(_locomotive);
			Assert.IsNotNull(_cowController);
		}
		public void StartStopStateChanged(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				_locomotive.StartStopStateChanged();
			}
		}

		public void DecreaseSpeedStateChanged(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				_locomotive.DecreaseSpeedStateChanged();
			}
		}

		public void IncreaseSpeedStateChanged(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				_locomotive.IncreaseSpeedStateChanged();
			}
		}

		public void SmokeButtonStateChanged(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				_locomotive.SmokeButtonStateChanged();
			}
		}

		public void WhistleButtonStateChanged(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				_locomotive.WhistleButtonStateChanged();
			}
		}

		public void ReverseButtonStateChanged(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				_locomotive.ReverseButtonStateChanged();
			}
		}

		public void ChangeState(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				CollisionHandlerObject.currentInteraction = (HandleCube.interactionState)(((int)(CollisionHandlerObject.currentInteraction) + 1) % 3);
			}
		}

		public void GoMoo(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				_cowController.GoMooCowGo();
			}
		}

		public void Spawn(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				Debug.Log("aaaaaaaaa");
			}
		}

		public void Select(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				if (isFixed)
					return;
			}
		}

		public void RotYplus(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				if (isFixed)
					return;
			}
		}

		public void RotYminus(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				if (isFixed)
					return;
			}
		}

		public void RotZplus(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				if (isFixed)
					return;
			}
		}

		public void RotZminus(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				if (isFixed)
					return;
			}
		}

		public void RotXplus(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				if (isFixed)
					return;
			}
		}

		public void RotXminus(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				if (isFixed)
					return;
			}
		}


		public void RecalculatePos(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				if (isFixed)
					return;
			}
		}

		public void Fix(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				if (isFixed)
					return;
			}
		}
	}
}
