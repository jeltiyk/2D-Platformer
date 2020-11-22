using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ActionsController))]
public class Throw : MonoBehaviour
{
    private ActionsController _actionsController;

    private enum ThrowMode
    {
        Throw,
        MultipleThrow,
        ThrowInRange
    }
    
    [Header("Throwing position")]
    [SerializeField] private Transform position;

    [Header("Throw")] 
    [SerializeField] private GameObject @object;
    
    [Header("Throw multiple")]
    [SerializeField] private GameObject multipleObject;
    
    [Header("Throw in range")]
    [SerializeField] private GameObject objectRange;
    [SerializeField] private int countOfObjects = 3;
    private int _angularRange;
    private const int AngularMultiplier = 3;

    [Header("Object settings")]
    [SerializeField] private float objectSpeed = 13f;
    
    [Header("Key hold time")]
    [SerializeField] private float maxTime = 0.8f;
    [SerializeField] private float minTime = 0.2f;
    // keyHoldTime / period => count of objects 
    [Tooltip("(Key hold time / period) => count of objects")]
    [SerializeField] private float period = 0.15f;

    [Header("Throw frequency")]
    [SerializeField] private float frequency = 0.08f;
    
    [Header("Destroy object after")]
    [SerializeField] private float time = 3f;

    #region Properties

    public float MAXTime => maxTime;
    public float MINTime => minTime;

    #endregion
    
    private void Awake()
    {
        _actionsController = GetComponent<ActionsController>();
        _angularRange = countOfObjects * AngularMultiplier;
    }

    public void ThrowObject()
    {
        StartCoroutine(ObjectsController(ThrowMode.Throw));
    }
    
    public void ThrowMultipleObjects(float keyHoldTime)
    {
        StartCoroutine(ObjectsController(ThrowMode.MultipleThrow,(int)(keyHoldTime / period)));
    }

    public void ThrowObjectsInRange()
    {
        StartCoroutine(ObjectsController(ThrowMode.ThrowInRange));
    }
    public void ThrowObjectsInRange(float keyHoldTime)
    {
        StartCoroutine(ObjectsController(ThrowMode.ThrowInRange, (int)(keyHoldTime / period)));
    }

    private IEnumerator ObjectsController(ThrowMode mode, int countObjects = 1)
    {
        switch (mode)
        {
            case ThrowMode.Throw:

                if (_actionsController.PlayerController.ThrowableObjects > 0)
                {
                    GameObject thrownObject = Instantiate(multipleObject, position.position, Quaternion.identity);
                    thrownObject.GetComponent<Rigidbody2D>().AddForce(transform.right * objectSpeed, ForceMode2D.Impulse);

                    _actionsController.PlayerController.ThrowableObjects--;
                
                    Destroy(thrownObject, time);
                    
                    yield return new WaitForSeconds(frequency);
                }
                
                break;
            case ThrowMode.MultipleThrow:
                
                if (_actionsController.PlayerController.ThrowableObjects < countObjects)
                    countObjects = _actionsController.PlayerController.ThrowableObjects;
                
                while (countObjects > 0)
                {
                    GameObject thrownObjects = Instantiate(multipleObject, position.position, Quaternion.identity);
                    thrownObjects.GetComponent<Rigidbody2D>().AddForce(transform.right * objectSpeed, ForceMode2D.Impulse);
    
                    countObjects--;
                    _actionsController.PlayerController.ThrowableObjects--;
                
                    Destroy(thrownObjects, time);
                    
                    yield return new WaitForSeconds(frequency);
                }
    
                break;
            case ThrowMode.ThrowInRange:
                
                if (_actionsController.PlayerController.ThrowableObjects >= countObjects * countOfObjects)
                {
                    while (countObjects > 0)
                    {
                        int objects;
                        GameObject[] thrownObjects;
    
                        if (countOfObjects % 2 == 0 && countOfObjects != 0)
                        {
                            thrownObjects = new GameObject[countOfObjects - 1];
                            objects = countOfObjects - 2;
                        }
                        else
                        {
                            thrownObjects = new GameObject[countOfObjects];
                            objects = countOfObjects - 1;
                        }
    
                        int angleStep = _angularRange / (objects + 1);
                        int upperAngularBorder = ((objects + 1) / 2) * angleStep;
                        int currentAngle = upperAngularBorder;
    
                        #region Legacy
    
                        //int angularStep = actionsController.RangeAngle / (countOfObjects + 1);
                        //int upperAngularBorder = ((countOfObjects + 1) / 2) * step;
                        //int lowerAngularBorder = -upperAngularBorder;
    
                        //float[] angularCoefficients = new float[actionsController.MaxCountObjects];
    
                        //for (int i = upperAngularBorder, j = 0; i >= lowerAngularBorder; i -= angularStep, j++)
                        //{
                        //    float k = (float)Math.Tan(i);
                        //    angularCoefficients[j] = k;
                        //}
    
                        #endregion
    
                        while (objects >= 0)
                        {
                            thrownObjects[objects] =
                                Instantiate(objectRange, position.position,
                                    Quaternion.identity);
                            thrownObjects[objects].GetComponent<Rigidbody2D>()
                                .AddForce(
                                    (transform.right + transform.up * Mathf.Tan(currentAngle)) *
                                    objectSpeed,
                                    ForceMode2D.Impulse);
    
                            #region Legacy
    
                            //thrownObjects[objects].GetComponent<Rigidbody2D>().AddForce
                            //    ((transform.right + transform.up * angularCoefficients[objects]) * objectSpeed, ForceMode2D.Impulse);
    
                            #endregion
                            
                            Destroy(thrownObjects[objects], time);
                            
                            objects--;
                            currentAngle -= angleStep;
                            _actionsController.PlayerController.ThrowableObjects--;
                        }
    
                        countObjects--;
    
                        yield return new WaitForSeconds(frequency);
                    }
                }
                
                break;
        }
    }
}
