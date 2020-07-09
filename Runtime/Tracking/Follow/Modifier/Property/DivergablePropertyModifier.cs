﻿namespace Zinnia.Tracking.Follow.Modifier.Property
{
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.XmlDocumentationAttribute;
    using System.Collections.Generic;
    using UnityEngine;
    using Zinnia.Extension;

    /// <summary>
    /// Modifies a property by a mechanism that can cause the target to diverge from the source.
    /// </summary>
    public abstract class DivergablePropertyModifier : PropertyModifier
    {
        #region Divergence Events
        /// <summary>
        /// Emitted when the target is back within the threshold distance of the source after being diverged.
        /// </summary>
        [Header("Divergence Events"), DocumentedByXml]
        public ObjectFollower.FollowEvent Converged = new ObjectFollower.FollowEvent();
        /// <summary>
        /// Emitted when the target is no longer within the threshold distance of the source.
        /// </summary>
        [DocumentedByXml]
        public ObjectFollower.FollowEvent Diverged = new ObjectFollower.FollowEvent();
        #endregion

        #region Divergence Settings
        /// <summary>
        /// Determines if to track whether the source diverges from the target. Tracking divergence adds additional overhead.
        /// </summary>
        [Serialized]
        [field: Header("Divergence Settings"), DocumentedByXml]
        public bool TrackDivergence { get; set; }
        /// <summary>
        /// The distance the target has to be away from the source to be considered diverged.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public Vector3 DivergenceThreshold { get; set; } = Vector3.one * 0.1f;
        #endregion

        /// <summary>
        /// A collection of currently diverged states.
        /// </summary>
        protected HashSet<string> divergedStates = new HashSet<string>();

        /// <summary>
        /// Sets the <see cref="DivergenceThreshold"/> x value.
        /// </summary>
        /// <param name="value">The value to set to.</param>
        public virtual void SetDivergenceThresholdX(float value)
        {
            DivergenceThreshold = new Vector3(value, DivergenceThreshold.y, DivergenceThreshold.z);
        }

        /// <summary>
        /// Sets the <see cref="DivergenceThreshold"/> y value.
        /// </summary>
        /// <param name="value">The value to set to.</param>
        public virtual void SetDivergenceThresholdY(float value)
        {
            DivergenceThreshold = new Vector3(DivergenceThreshold.x, value, DivergenceThreshold.z);
        }

        /// <summary>
        /// Sets the <see cref="DivergenceThreshold"/> z value.
        /// </summary>
        /// <param name="value">The value to set to.</param>
        public virtual void SetDivergenceThresholdZ(float value)
        {
            DivergenceThreshold = new Vector3(DivergenceThreshold.x, DivergenceThreshold.y, value);
        }

        /// <summary>
        /// Whether the given source and target are diverged.
        /// </summary>
        /// <param name="source">The source to check against.</param>
        /// <param name="target">The target to check with.</param>
        /// <returns>Whether a divergence is occurring.</returns>
        public virtual bool AreDiverged(GameObject source, GameObject target)
        {
            return divergedStates.Contains(GenerateIdentifier(source, target));
        }

        /// <summary>
        /// Gets the two points from the source and target to check divergence against.
        /// </summary>
        /// <param name="source">The source to check against.</param>
        /// <param name="target">The target to check with.</param>
        /// <param name="offset">Any offset applied to the target.</param>
        /// <param name="a">The first point to use.</param>
        /// <param name="b">The second point to use.</param>
        protected abstract void GetCheckPoints(GameObject source, GameObject target, GameObject offset, out Vector3 a, out Vector3 b);

        /// <inheritdoc/>
        protected override void DoModify(GameObject source, GameObject target, GameObject offset = null)
        {
            if (TrackDivergence)
            {
                CheckDivergence(source, target, offset);
            }
        }

        /// <summary>
        /// Generates the unique key for the divergence check.
        /// </summary>
        /// <param name="source">The source of the divergence check.</param>
        /// <param name="target">The target of the divergence check.</param>
        /// <returns>The unique identifier.</returns>
        protected virtual string GenerateIdentifier(GameObject source, GameObject target)
        {
            return source.GetInstanceID() + "-" + target.GetInstanceID();
        }

        /// <summary>
        /// Checks to see if the target has diverged from the source within the <see cref="DivergenceThreshold"/>.
        /// </summary>
        /// <param name="source">The source to check against.</param>
        /// <param name="target">The target to check with.</param>
        /// <param name="offset">Any offset applied to the target.</param>
        protected virtual void CheckDivergence(GameObject source, GameObject target, GameObject offset)
        {
            string divergeKey = GenerateIdentifier(source, target);
            bool areDiverged = AreDiverged(source, target);
            GetCheckPoints(source, target, offset, out Vector3 sourcePoint, out Vector3 targetPoint);

            if (!sourcePoint.WithinDistance(targetPoint, DivergenceThreshold))
            {
                if (areDiverged)
                {
                    return;
                }

                divergedStates.Add(divergeKey);
                Diverged?.Invoke(eventData.Set(source, target, offset));
            }
            else if (areDiverged)
            {
                divergedStates.Remove(divergeKey);
                Converged?.Invoke(eventData.Set(source, target, offset));
            }
        }
    }
}