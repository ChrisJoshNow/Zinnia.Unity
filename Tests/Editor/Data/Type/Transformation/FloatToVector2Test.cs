﻿using VRTK.Core.Data.Type.Transformation;

namespace Test.VRTK.Core.Data.Type.Transformation
{
    using UnityEngine;
    using NUnit.Framework;
    using Test.VRTK.Core.Utility.Mock;

    public class FloatToVector2Test
    {
        private GameObject containingObject;
        private FloatToVector2 subject;

        [SetUp]
        public void SetUp()
        {
            containingObject = new GameObject();
            subject = containingObject.AddComponent<FloatToVector2>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(subject);
            Object.DestroyImmediate(containingObject);
        }

        [Test]
        public void Transform()
        {
            UnityEventListenerMock transformedListenerMock = new UnityEventListenerMock();
            subject.Transformed.AddListener(transformedListenerMock.Listen);

            Assert.AreEqual(Vector2.zero, subject.Result);
            Assert.IsFalse(transformedListenerMock.Received);

            float[] input = new float[] { 2f, 3f };
            Vector2 result = subject.Transform(input);
            Vector2 expectedResult = new Vector2(2f, 3f);

            Assert.AreEqual(expectedResult, result);
            Assert.AreEqual(expectedResult, subject.Result);
            Assert.IsTrue(transformedListenerMock.Received);
        }

        [Test]
        public void TransformWithSet()
        {
            UnityEventListenerMock transformedListenerMock = new UnityEventListenerMock();
            subject.Transformed.AddListener(transformedListenerMock.Listen);

            Assert.AreEqual(Vector2.zero, subject.Result);
            Assert.IsFalse(transformedListenerMock.Received);

            subject.SetX(2f);
            subject.SetY(3f);
            Vector2 result = subject.Transform();
            Vector2 expectedResult = new Vector2(2f, 3f);

            Assert.AreEqual(expectedResult, result);
            Assert.AreEqual(expectedResult, subject.Result);
            Assert.IsTrue(transformedListenerMock.Received);
        }

        [Test]
        public void TransformInactiveGameObject()
        {
            UnityEventListenerMock transformedListenerMock = new UnityEventListenerMock();
            subject.Transformed.AddListener(transformedListenerMock.Listen);
            subject.gameObject.SetActive(false);

            Assert.AreEqual(Vector2.zero, subject.Result);
            Assert.IsFalse(transformedListenerMock.Received);

            float[] input = new float[] { 2f, 3f };
            Vector2 result = subject.Transform(input);

            Assert.AreEqual(Vector2.zero, result);
            Assert.AreEqual(Vector2.zero, subject.Result);
            Assert.IsFalse(transformedListenerMock.Received);
        }

        [Test]
        public void TransformInactiveComponent()
        {
            UnityEventListenerMock transformedListenerMock = new UnityEventListenerMock();
            subject.Transformed.AddListener(transformedListenerMock.Listen);
            subject.enabled = false;

            Assert.AreEqual(Vector2.zero, subject.Result);
            Assert.IsFalse(transformedListenerMock.Received);

            float[] input = new float[] { 2f, 3f };
            Vector2 result = subject.Transform(input);

            Assert.AreEqual(Vector2.zero, result);
            Assert.AreEqual(Vector2.zero, subject.Result);
            Assert.IsFalse(transformedListenerMock.Received);
        }
    }
}