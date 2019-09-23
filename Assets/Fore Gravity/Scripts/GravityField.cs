using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class GravityField : MonoBehaviour
{
    #pragma warning disable 0649
    [SerializeField] private AnimationCurve _ForceCurve;
    [SerializeField] private float _MaxForce = 0.5f;
    [SerializeField] private LayerMask _ObjectMask = 1 << 8;
    [SerializeField] private float increaseFactor = 1.1f;
    [SerializeField] private float maxParticleSize = 0.1f;
    #pragma warning restore 0649

    private SphereCollider _Collider;

    private void Start()
    {
        _Collider = GetComponent<SphereCollider>();
        _Collider.isTrigger = true;
    }

    void OnTriggerStay(Collider other) {
        var destObj = other.GetComponent<DestroyableObj>();
        if (destObj == null)
        {
            return;
        }

        int threshold = destObj.GetThreshold();

        // If other object is not in Destroyable Object layer (8)
        if ((_ObjectMask.value & (1 << other.gameObject.layer)) == 0)
        {
            return;
        }

        // If we are strong enough to pick up the other object
        if (GameManager.S.GetDestroyedScore() >= threshold) {
            // Vector faces towards the center of the gravity field
            var vec = transform.position - other.transform.position;

            // Normalize the magnitude b/w 0 and 1 to put on the force curve
            float normDist = vec.magnitude / _Collider.radius;
            if (vec.magnitude > _Collider.radius) normDist = 1;

            other.transform.position = Vector3.MoveTowards(other.transform.position, this.transform.position, _ForceCurve.Evaluate(normDist) * _MaxForce);

            // Decrease other object's size exponentially
            // TODO: why does size increase?
            if (vec.magnitude < 1) {
                other.transform.localScale *= vec.magnitude;
            }
        }
    }

    public void IncreaseGravityField(){
        GetComponent<SphereCollider>().radius *= increaseFactor;
        ParticleSystem.ShapeModule shapeModule = transform.GetChild(0).GetComponent <ParticleSystem> ().shape;

        // Normalize increase of particle system
        // var increase = shapeModule.radius * increaseFactor;
        // var normalized = increase > 1 ? 1/increase : increase;
        // shapeModule.radius = normalized * maxParticleSize;
    }

    // Draws a debug view of a sphere where the gravity field is. Does not show
    // up in actual game.
    private void OnDrawGizmos()
    {
        _Collider = GetComponent<SphereCollider>();
        if (_Collider == null)
        {
            return;
        }

        Gizmos.color = new Color(0, 1, 0, 0.4f);
        Gizmos.DrawSphere(transform.position, _Collider.radius);
    }
}
