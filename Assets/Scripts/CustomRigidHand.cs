/******************************************************************************
 * Copyright (C) Ultraleap, Inc. 2011-2020.                                   *
 *                                                                            *
 * Use subject to the terms of the Apache License 2.0 available at            *
 * http://www.apache.org/licenses/LICENSE-2.0, or another agreement           *
 * between Ultraleap and you, your company or other organization.             *
 ******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;

namespace Leap.Unity
{
    /** A physics model for our rigid hand made out of various Unity Collider. */
    public class CustomRigidHand : SkeletalHand
    {
        public override ModelType HandModelType
        {
            get
            {
                return ModelType.Physics;
            }
        }
        public float filtering = 0.5f;

        public override bool SupportsEditorPersistence()
        {
            return true;
        }

        public override void InitHand()
        {
            base.InitHand();
        }

        public override void UpdateHand()
        {

            for (int f = 0; f < fingers.Length; ++f)
            {
                if (fingers[f] != null)
                {
                    fingers[f].UpdateFinger();
                }
            }

            if (palm != null)
            {
                Rigidbody palmBody = palm.GetComponent<Rigidbody>();
                if (palmBody)
                {
                    palmBody.MovePosition(GetPalmCenter());
                    palmBody.MoveRotation(GetPalmRotation());
                }
                else
                {
                    palm.position = GetPalmCenter();
                    palm.rotation = GetPalmRotation();
                }
            }
        }
    }
}
