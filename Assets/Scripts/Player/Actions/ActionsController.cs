using UnityEngine;

[RequireComponent(typeof(PlayerController))]
class ActionsController : MonoBehaviour
{
    [Header("Actions")]
    [SerializeField] private bool @throw;
    [SerializeField] private bool throwMultiple;
    [SerializeField] private bool throwInRange;
    
    [Header("Ignore hold time for throw in range")] 
    [SerializeField] private bool ignore;
    
    #region Properties
    
    public bool Ignore => ignore;
    public Throw Throw { get; private set; }
    public PlayerController PlayerController { get; private set; }

    #endregion
    
    private void Awake()
    {
        Throw = GetComponent<Throw>();
        PlayerController = GetComponent<PlayerController>();
    }

    public void Actions(bool @throw, bool inRange, float keyHoldTime)
    {
        if (@throw)
        {
            if(this.@throw && keyHoldTime <= Throw.MINTime)
                Throw.ThrowObject();
            
            if(throwMultiple && keyHoldTime > Throw.MINTime)
                Throw.ThrowMultipleObjects(keyHoldTime);
        }
        else if (inRange)
        {
            if (ignore)
            {
                Throw.ThrowObjectsInRange();
                //return;
            }
            
            if(throwMultiple && throwInRange)
                Throw.ThrowObjectsInRange(keyHoldTime);
        }
    }
}
