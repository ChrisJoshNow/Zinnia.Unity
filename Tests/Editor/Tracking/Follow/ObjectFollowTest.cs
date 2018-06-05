﻿namespace VRTK.Core.Tracking.Follow
{
    using UnityEngine;
    using NUnit.Framework;
    using VRTK.Core.Tracking.Follow.Modifier;
    using VRTK.Core.Utility.Mock;

    public class ObjectFollowTest
    {
        private GameObject containingObject;
        private ObjectFollow subject;

        [SetUp]
        public void SetUp()
        {
            containingObject = new GameObject();
            subject = containingObject.AddComponent<ObjectFollow>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(subject);
            Object.DestroyImmediate(containingObject);
        }

        [Test]
        public void ProcessFirstActiveTargetAllActive()
        {
            GameObject source = new GameObject("source");
            GameObject[] targets = new GameObject[]
            {
                new GameObject("target1"),
                new GameObject("target2"),
                new GameObject("target3")
            };

            subject.sourceComponent = source.transform;
            subject.targetComponents = new Component[]
            {
                targets[0].transform, targets[1].transform, targets[2].transform
            };

            FollowModifierMock followModifierMock = containingObject.AddComponent<FollowModifierMock>();
            followModifierMock.processFirstAndActiveOnly = true;

            subject.follow = Data.Enum.TransformProperties.Position;
            subject.followModifier = followModifierMock;

            subject.Process();

            Assert.AreEqual(Vector3.one, targets[0].transform.position);
            Assert.AreEqual(Vector3.zero, targets[1].transform.position);
            Assert.AreEqual(Vector3.zero, targets[2].transform.position);
        }

        [Test]
        public void ProcessFirstActiveTargetOnlyLastActive()
        {
            GameObject source = new GameObject("source");
            GameObject[] targets = new GameObject[]
            {
                new GameObject("target1"),
                new GameObject("target2"),
                new GameObject("target3")
            };

            subject.sourceComponent = source.transform;
            subject.targetComponents = new Component[]
            {
                targets[0].transform, targets[1].transform, targets[2].transform
            };

            FollowModifierMock followModifierMock = containingObject.AddComponent<FollowModifierMock>();
            followModifierMock.processFirstAndActiveOnly = true;

            subject.follow = Data.Enum.TransformProperties.Position;
            subject.followModifier = followModifierMock;

            targets[0].SetActive(false);
            targets[1].SetActive(false);

            subject.Process();

            Assert.AreEqual(Vector3.zero, targets[0].transform.position);
            Assert.AreEqual(Vector3.zero, targets[1].transform.position);
            Assert.AreEqual(Vector3.one, targets[2].transform.position);
        }

        [Test]
        public void ProcessAllTargets()
        {
            UnityEventListenerMock beforeProcessedMock = new UnityEventListenerMock();
            UnityEventListenerMock afterProcessedMock = new UnityEventListenerMock();
            UnityEventListenerMock beforeTransformUpdatedMock = new UnityEventListenerMock();
            UnityEventListenerMock afterTransformUpdatedMock = new UnityEventListenerMock();

            subject.BeforeProcessed.AddListener(beforeProcessedMock.Listen);
            subject.AfterProcessed.AddListener(afterProcessedMock.Listen);
            subject.BeforeTransformUpdated.AddListener(beforeTransformUpdatedMock.Listen);
            subject.AfterTransformUpdated.AddListener(afterTransformUpdatedMock.Listen);

            GameObject source = new GameObject("source");
            GameObject[] targets = new GameObject[]
            {
                new GameObject("target1"),
                new GameObject("target2"),
                new GameObject("target3")
            };

            subject.sourceComponent = source.transform;
            subject.targetComponents = new Component[]
            {
                targets[0].transform, targets[1].transform, targets[2].transform
            };

            FollowModifierMock followModifierMock = containingObject.AddComponent<FollowModifierMock>();
            followModifierMock.processFirstAndActiveOnly = false;

            subject.follow = Data.Enum.TransformProperties.Position;
            subject.followModifier = followModifierMock;

            subject.Process();

            Assert.AreEqual(Vector3.one, targets[0].transform.position);
            Assert.AreEqual(Vector3.one, targets[1].transform.position);
            Assert.AreEqual(Vector3.one, targets[2].transform.position);

            Assert.IsTrue(beforeProcessedMock.Received);
            Assert.IsTrue(afterProcessedMock.Received);
            Assert.IsTrue(beforeTransformUpdatedMock.Received);
            Assert.IsTrue(afterTransformUpdatedMock.Received);
        }

        [Test]
        public void ProcessPositionOnly()
        {
            UnityEventListenerMock beforePositionUpdatedMock = new UnityEventListenerMock();
            UnityEventListenerMock afterPositionUpdatedMock = new UnityEventListenerMock();
            UnityEventListenerMock beforeRotationUpdatedMock = new UnityEventListenerMock();
            UnityEventListenerMock afterRotationUpdatedMock = new UnityEventListenerMock();
            UnityEventListenerMock beforeScaleUpdatedMock = new UnityEventListenerMock();
            UnityEventListenerMock afterScaleUpdatedMock = new UnityEventListenerMock();

            subject.BeforePositionUpdated.AddListener(beforePositionUpdatedMock.Listen);
            subject.AfterPositionUpdated.AddListener(afterPositionUpdatedMock.Listen);
            subject.BeforeRotationUpdated.AddListener(beforeRotationUpdatedMock.Listen);
            subject.AfterRotationUpdated.AddListener(afterRotationUpdatedMock.Listen);
            subject.BeforeScaleUpdated.AddListener(beforeScaleUpdatedMock.Listen);
            subject.AfterScaleUpdated.AddListener(afterScaleUpdatedMock.Listen);

            GameObject source = new GameObject("source");
            GameObject[] targets = new GameObject[]
            {
                new GameObject("target1"),
                new GameObject("target2"),
                new GameObject("target3")
            };

            subject.sourceComponent = source.transform;
            subject.targetComponents = new Component[]
            {
                targets[0].transform, targets[1].transform, targets[2].transform
            };

            FollowModifierMock followModifierMock = containingObject.AddComponent<FollowModifierMock>();
            followModifierMock.processFirstAndActiveOnly = false;

            subject.follow = Data.Enum.TransformProperties.Position;
            subject.followModifier = followModifierMock;

            subject.Process();

            Assert.AreEqual(Vector3.one, targets[0].transform.position);
            Assert.AreEqual(Vector3.one, targets[1].transform.position);
            Assert.AreEqual(Vector3.one, targets[2].transform.position);

            Assert.AreEqual(Quaternion.identity, targets[0].transform.rotation);
            Assert.AreEqual(Quaternion.identity, targets[1].transform.rotation);
            Assert.AreEqual(Quaternion.identity, targets[2].transform.rotation);

            Assert.AreEqual(Vector3.one, targets[0].transform.localScale);
            Assert.AreEqual(Vector3.one, targets[1].transform.localScale);
            Assert.AreEqual(Vector3.one, targets[2].transform.localScale);

            Assert.IsTrue(beforePositionUpdatedMock.Received);
            Assert.IsTrue(afterPositionUpdatedMock.Received);
            Assert.IsFalse(beforeRotationUpdatedMock.Received);
            Assert.IsFalse(afterRotationUpdatedMock.Received);
            Assert.IsFalse(beforeScaleUpdatedMock.Received);
            Assert.IsFalse(afterScaleUpdatedMock.Received);
        }

        [Test]
        public void ProcessRotationOnly()
        {
            UnityEventListenerMock beforePositionUpdatedMock = new UnityEventListenerMock();
            UnityEventListenerMock afterPositionUpdatedMock = new UnityEventListenerMock();
            UnityEventListenerMock beforeRotationUpdatedMock = new UnityEventListenerMock();
            UnityEventListenerMock afterRotationUpdatedMock = new UnityEventListenerMock();
            UnityEventListenerMock beforeScaleUpdatedMock = new UnityEventListenerMock();
            UnityEventListenerMock afterScaleUpdatedMock = new UnityEventListenerMock();

            subject.BeforePositionUpdated.AddListener(beforePositionUpdatedMock.Listen);
            subject.AfterPositionUpdated.AddListener(afterPositionUpdatedMock.Listen);
            subject.BeforeRotationUpdated.AddListener(beforeRotationUpdatedMock.Listen);
            subject.AfterRotationUpdated.AddListener(afterRotationUpdatedMock.Listen);
            subject.BeforeScaleUpdated.AddListener(beforeScaleUpdatedMock.Listen);
            subject.AfterScaleUpdated.AddListener(afterScaleUpdatedMock.Listen);

            GameObject source = new GameObject("source");
            GameObject[] targets = new GameObject[]
            {
                new GameObject("target1"),
                new GameObject("target2"),
                new GameObject("target3")
            };

            subject.sourceComponent = source.transform;
            subject.targetComponents = new Component[]
            {
                targets[0].transform, targets[1].transform, targets[2].transform
            };

            FollowModifierMock followModifierMock = containingObject.AddComponent<FollowModifierMock>();
            followModifierMock.processFirstAndActiveOnly = false;

            subject.follow = Data.Enum.TransformProperties.Rotation;
            subject.followModifier = followModifierMock;

            subject.Process();

            Quaternion expectedRotation = new Quaternion(1f, 0f, 0f, 0f);

            Assert.AreEqual(Vector3.zero, targets[0].transform.position);
            Assert.AreEqual(Vector3.zero, targets[1].transform.position);
            Assert.AreEqual(Vector3.zero, targets[2].transform.position);

            Assert.AreEqual(expectedRotation, targets[0].transform.rotation);
            Assert.AreEqual(expectedRotation, targets[1].transform.rotation);
            Assert.AreEqual(expectedRotation, targets[2].transform.rotation);

            Assert.AreEqual(Vector3.one, targets[0].transform.localScale);
            Assert.AreEqual(Vector3.one, targets[1].transform.localScale);
            Assert.AreEqual(Vector3.one, targets[2].transform.localScale);

            Assert.IsFalse(beforePositionUpdatedMock.Received);
            Assert.IsFalse(afterPositionUpdatedMock.Received);
            Assert.IsTrue(beforeRotationUpdatedMock.Received);
            Assert.IsTrue(afterRotationUpdatedMock.Received);
            Assert.IsFalse(beforeScaleUpdatedMock.Received);
            Assert.IsFalse(afterScaleUpdatedMock.Received);
        }

        [Test]
        public void ProcessScaleOnly()
        {
            UnityEventListenerMock beforePositionUpdatedMock = new UnityEventListenerMock();
            UnityEventListenerMock afterPositionUpdatedMock = new UnityEventListenerMock();
            UnityEventListenerMock beforeRotationUpdatedMock = new UnityEventListenerMock();
            UnityEventListenerMock afterRotationUpdatedMock = new UnityEventListenerMock();
            UnityEventListenerMock beforeScaleUpdatedMock = new UnityEventListenerMock();
            UnityEventListenerMock afterScaleUpdatedMock = new UnityEventListenerMock();

            subject.BeforePositionUpdated.AddListener(beforePositionUpdatedMock.Listen);
            subject.AfterPositionUpdated.AddListener(afterPositionUpdatedMock.Listen);
            subject.BeforeRotationUpdated.AddListener(beforeRotationUpdatedMock.Listen);
            subject.AfterRotationUpdated.AddListener(afterRotationUpdatedMock.Listen);
            subject.BeforeScaleUpdated.AddListener(beforeScaleUpdatedMock.Listen);
            subject.AfterScaleUpdated.AddListener(afterScaleUpdatedMock.Listen);

            GameObject source = new GameObject("source");
            GameObject[] targets = new GameObject[]
            {
                new GameObject("target1"),
                new GameObject("target2"),
                new GameObject("target3")
            };

            subject.sourceComponent = source.transform;
            subject.targetComponents = new Component[]
            {
                targets[0].transform, targets[1].transform, targets[2].transform
            };

            FollowModifierMock followModifierMock = containingObject.AddComponent<FollowModifierMock>();
            followModifierMock.processFirstAndActiveOnly = false;

            subject.follow = Data.Enum.TransformProperties.Scale;
            subject.followModifier = followModifierMock;

            subject.Process();

            Vector3 expectedScale = new Vector3(2f, 2f, 2f);

            Assert.AreEqual(Vector3.zero, targets[0].transform.position);
            Assert.AreEqual(Vector3.zero, targets[1].transform.position);
            Assert.AreEqual(Vector3.zero, targets[2].transform.position);

            Assert.AreEqual(Quaternion.identity, targets[0].transform.rotation);
            Assert.AreEqual(Quaternion.identity, targets[1].transform.rotation);
            Assert.AreEqual(Quaternion.identity, targets[2].transform.rotation);

            Assert.AreEqual(expectedScale, targets[0].transform.localScale);
            Assert.AreEqual(expectedScale, targets[1].transform.localScale);
            Assert.AreEqual(expectedScale, targets[2].transform.localScale);

            Assert.IsFalse(beforePositionUpdatedMock.Received);
            Assert.IsFalse(afterPositionUpdatedMock.Received);
            Assert.IsFalse(beforeRotationUpdatedMock.Received);
            Assert.IsFalse(afterRotationUpdatedMock.Received);
            Assert.IsTrue(beforeScaleUpdatedMock.Received);
            Assert.IsTrue(afterScaleUpdatedMock.Received);
        }

        [Test]
        public void ProcessPositionAndRotationOnly()
        {
            UnityEventListenerMock beforePositionUpdatedMock = new UnityEventListenerMock();
            UnityEventListenerMock afterPositionUpdatedMock = new UnityEventListenerMock();
            UnityEventListenerMock beforeRotationUpdatedMock = new UnityEventListenerMock();
            UnityEventListenerMock afterRotationUpdatedMock = new UnityEventListenerMock();
            UnityEventListenerMock beforeScaleUpdatedMock = new UnityEventListenerMock();
            UnityEventListenerMock afterScaleUpdatedMock = new UnityEventListenerMock();

            subject.BeforePositionUpdated.AddListener(beforePositionUpdatedMock.Listen);
            subject.AfterPositionUpdated.AddListener(afterPositionUpdatedMock.Listen);
            subject.BeforeRotationUpdated.AddListener(beforeRotationUpdatedMock.Listen);
            subject.AfterRotationUpdated.AddListener(afterRotationUpdatedMock.Listen);
            subject.BeforeScaleUpdated.AddListener(beforeScaleUpdatedMock.Listen);
            subject.AfterScaleUpdated.AddListener(afterScaleUpdatedMock.Listen);

            GameObject source = new GameObject("source");
            GameObject[] targets = new GameObject[]
            {
                new GameObject("target1"),
                new GameObject("target2"),
                new GameObject("target3")
            };

            subject.sourceComponent = source.transform;
            subject.targetComponents = new Component[]
            {
                targets[0].transform, targets[1].transform, targets[2].transform
            };

            FollowModifierMock followModifierMock = containingObject.AddComponent<FollowModifierMock>();
            followModifierMock.processFirstAndActiveOnly = false;

            subject.follow = Data.Enum.TransformProperties.Position | Data.Enum.TransformProperties.Rotation;
            subject.followModifier = followModifierMock;

            subject.Process();

            Quaternion expectedRotation = new Quaternion(1f, 0f, 0f, 0f);

            Assert.AreEqual(Vector3.one, targets[0].transform.position);
            Assert.AreEqual(Vector3.one, targets[1].transform.position);
            Assert.AreEqual(Vector3.one, targets[2].transform.position);

            Assert.AreEqual(expectedRotation, targets[0].transform.rotation);
            Assert.AreEqual(expectedRotation, targets[1].transform.rotation);
            Assert.AreEqual(expectedRotation, targets[2].transform.rotation);

            Assert.AreEqual(Vector3.one, targets[0].transform.localScale);
            Assert.AreEqual(Vector3.one, targets[1].transform.localScale);
            Assert.AreEqual(Vector3.one, targets[2].transform.localScale);

            Assert.IsTrue(beforePositionUpdatedMock.Received);
            Assert.IsTrue(afterPositionUpdatedMock.Received);
            Assert.IsTrue(beforeRotationUpdatedMock.Received);
            Assert.IsTrue(afterRotationUpdatedMock.Received);
            Assert.IsFalse(beforeScaleUpdatedMock.Received);
            Assert.IsFalse(afterScaleUpdatedMock.Received);
        }

        [Test]
        public void EventsNotEmittedOnInactiveGameObject()
        {
            UnityEventListenerMock beforeProcessedMock = new UnityEventListenerMock();
            UnityEventListenerMock afterProcessedMock = new UnityEventListenerMock();
            UnityEventListenerMock beforeTransformUpdatedMock = new UnityEventListenerMock();
            UnityEventListenerMock afterTransformUpdatedMock = new UnityEventListenerMock();
            UnityEventListenerMock beforePositionUpdatedMock = new UnityEventListenerMock();
            UnityEventListenerMock afterPositionUpdatedMock = new UnityEventListenerMock();
            UnityEventListenerMock beforeRotationUpdatedMock = new UnityEventListenerMock();
            UnityEventListenerMock afterRotationUpdatedMock = new UnityEventListenerMock();
            UnityEventListenerMock beforeScaleUpdatedMock = new UnityEventListenerMock();
            UnityEventListenerMock afterScaleUpdatedMock = new UnityEventListenerMock();

            subject.BeforeProcessed.AddListener(beforeProcessedMock.Listen);
            subject.AfterProcessed.AddListener(afterProcessedMock.Listen);
            subject.BeforeTransformUpdated.AddListener(beforeTransformUpdatedMock.Listen);
            subject.AfterTransformUpdated.AddListener(afterTransformUpdatedMock.Listen);
            subject.BeforePositionUpdated.AddListener(beforePositionUpdatedMock.Listen);
            subject.AfterPositionUpdated.AddListener(afterPositionUpdatedMock.Listen);
            subject.BeforeRotationUpdated.AddListener(beforeRotationUpdatedMock.Listen);
            subject.AfterRotationUpdated.AddListener(afterRotationUpdatedMock.Listen);
            subject.BeforeScaleUpdated.AddListener(beforeScaleUpdatedMock.Listen);
            subject.AfterScaleUpdated.AddListener(afterScaleUpdatedMock.Listen);

            GameObject source = new GameObject("source");
            GameObject[] targets = new GameObject[]
            {
                new GameObject("target1"),
                new GameObject("target2"),
                new GameObject("target3")
            };

            subject.sourceComponent = source.transform;
            subject.targetComponents = new Component[]
            {
                targets[0].transform, targets[1].transform, targets[2].transform
            };

            FollowModifierMock followModifierMock = containingObject.AddComponent<FollowModifierMock>();
            followModifierMock.processFirstAndActiveOnly = false;

            subject.follow = Data.Enum.TransformProperties.Position | Data.Enum.TransformProperties.Rotation | Data.Enum.TransformProperties.Scale;
            subject.followModifier = followModifierMock;
            subject.gameObject.SetActive(false);

            subject.Process();

            Assert.AreEqual(Vector3.zero, targets[0].transform.position);
            Assert.AreEqual(Vector3.zero, targets[1].transform.position);
            Assert.AreEqual(Vector3.zero, targets[2].transform.position);

            Assert.AreEqual(Quaternion.identity, targets[0].transform.rotation);
            Assert.AreEqual(Quaternion.identity, targets[1].transform.rotation);
            Assert.AreEqual(Quaternion.identity, targets[2].transform.rotation);

            Assert.AreEqual(Vector3.one, targets[0].transform.localScale);
            Assert.AreEqual(Vector3.one, targets[1].transform.localScale);
            Assert.AreEqual(Vector3.one, targets[2].transform.localScale);

            Assert.IsFalse(beforeProcessedMock.Received);
            Assert.IsFalse(afterProcessedMock.Received);
            Assert.IsFalse(beforeTransformUpdatedMock.Received);
            Assert.IsFalse(afterTransformUpdatedMock.Received);
            Assert.IsFalse(beforePositionUpdatedMock.Received);
            Assert.IsFalse(afterPositionUpdatedMock.Received);
            Assert.IsFalse(beforeRotationUpdatedMock.Received);
            Assert.IsFalse(afterRotationUpdatedMock.Received);
            Assert.IsFalse(beforeScaleUpdatedMock.Received);
            Assert.IsFalse(afterScaleUpdatedMock.Received);
        }

        [Test]
        public void EventsNotEmittedOnDisabledComponent()
        {
            UnityEventListenerMock beforeProcessedMock = new UnityEventListenerMock();
            UnityEventListenerMock afterProcessedMock = new UnityEventListenerMock();
            UnityEventListenerMock beforeTransformUpdatedMock = new UnityEventListenerMock();
            UnityEventListenerMock afterTransformUpdatedMock = new UnityEventListenerMock();
            UnityEventListenerMock beforePositionUpdatedMock = new UnityEventListenerMock();
            UnityEventListenerMock afterPositionUpdatedMock = new UnityEventListenerMock();
            UnityEventListenerMock beforeRotationUpdatedMock = new UnityEventListenerMock();
            UnityEventListenerMock afterRotationUpdatedMock = new UnityEventListenerMock();
            UnityEventListenerMock beforeScaleUpdatedMock = new UnityEventListenerMock();
            UnityEventListenerMock afterScaleUpdatedMock = new UnityEventListenerMock();

            subject.BeforeProcessed.AddListener(beforeProcessedMock.Listen);
            subject.AfterProcessed.AddListener(afterProcessedMock.Listen);
            subject.BeforeTransformUpdated.AddListener(beforeTransformUpdatedMock.Listen);
            subject.AfterTransformUpdated.AddListener(afterTransformUpdatedMock.Listen);
            subject.BeforePositionUpdated.AddListener(beforePositionUpdatedMock.Listen);
            subject.AfterPositionUpdated.AddListener(afterPositionUpdatedMock.Listen);
            subject.BeforeRotationUpdated.AddListener(beforeRotationUpdatedMock.Listen);
            subject.AfterRotationUpdated.AddListener(afterRotationUpdatedMock.Listen);
            subject.BeforeScaleUpdated.AddListener(beforeScaleUpdatedMock.Listen);
            subject.AfterScaleUpdated.AddListener(afterScaleUpdatedMock.Listen);

            GameObject source = new GameObject("source");
            GameObject[] targets = new GameObject[]
            {
                new GameObject("target1"),
                new GameObject("target2"),
                new GameObject("target3")
            };

            subject.sourceComponent = source.transform;
            subject.targetComponents = new Component[]
            {
                targets[0].transform, targets[1].transform, targets[2].transform
            };

            FollowModifierMock followModifierMock = containingObject.AddComponent<FollowModifierMock>();
            followModifierMock.processFirstAndActiveOnly = false;

            subject.follow = Data.Enum.TransformProperties.Position | Data.Enum.TransformProperties.Rotation | Data.Enum.TransformProperties.Scale;
            subject.followModifier = followModifierMock;
            subject.enabled = false;

            subject.Process();

            Assert.AreEqual(Vector3.zero, targets[0].transform.position);
            Assert.AreEqual(Vector3.zero, targets[1].transform.position);
            Assert.AreEqual(Vector3.zero, targets[2].transform.position);

            Assert.AreEqual(Quaternion.identity, targets[0].transform.rotation);
            Assert.AreEqual(Quaternion.identity, targets[1].transform.rotation);
            Assert.AreEqual(Quaternion.identity, targets[2].transform.rotation);

            Assert.AreEqual(Vector3.one, targets[0].transform.localScale);
            Assert.AreEqual(Vector3.one, targets[1].transform.localScale);
            Assert.AreEqual(Vector3.one, targets[2].transform.localScale);

            Assert.IsFalse(beforeProcessedMock.Received);
            Assert.IsFalse(afterProcessedMock.Received);
            Assert.IsFalse(beforeTransformUpdatedMock.Received);
            Assert.IsFalse(afterTransformUpdatedMock.Received);
            Assert.IsFalse(beforePositionUpdatedMock.Received);
            Assert.IsFalse(afterPositionUpdatedMock.Received);
            Assert.IsFalse(beforeRotationUpdatedMock.Received);
            Assert.IsFalse(afterRotationUpdatedMock.Received);
            Assert.IsFalse(beforeScaleUpdatedMock.Received);
            Assert.IsFalse(afterScaleUpdatedMock.Received);
        }
    }

    public class FollowModifierMock : FollowModifier
    {
        public bool processFirstAndActiveOnly;

        public override bool ProcessFirstAndActiveOnly()
        {
            return processFirstAndActiveOnly;
        }

        public override void UpdatePosition(Transform source, Transform target)
        {
            target.position = Vector3.one;
        }

        public override void UpdateRotation(Transform source, Transform target)
        {
            target.rotation = new Quaternion(1f, 0f, 0f, 0f);
        }

        public override void UpdateScale(Transform source, Transform target)
        {
            target.localScale = new Vector3(2f, 2f, 2f);
        }
    }
}