using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyController : MonoBehaviour
{
    Transform _target;
    bool isChase = false;
    // Start is called before the first frame update
    void Start()
    {
        _target = GameObject.FindWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {

        if (isChase)
        {
            transform.LookAt(Vector3.Lerp(transform.forward + transform.position, _target.transform.position, 0.02f), Vector3.up);
        }
    }

    //ƒvƒŒƒCƒ„[‚ªõ“G”ÍˆÍ‚É“ü‚Á‚½‚Ìˆ—
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isChase = true;
        }
    }

    void CheckDistance()
    {
        float diff = (_target.position - transform.position).sqrMagnitude;
    }

}
