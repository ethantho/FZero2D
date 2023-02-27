
using UnityEngine;
using Fusion;

[ScriptHelp(BackColor = EditorHeaderBackColor.Steel)]
public class ControllerPrototype : Fusion.NetworkBehaviour {
  protected NetworkCharacterControllerPrototype _ncc;
  protected NetworkRigidbody _nrb;
  protected NetworkRigidbody2D _nrb2d;
  protected NetworkTransform _nt;


    public float acceleration;
    public float topSpeed;

    public float normAccel;
    public float boostAccel;

    public float normTopSpeed;
    public float boostTopSpeed;

    public float slipThreshold;
    public float slipAmount; //higher = higher traction

    public float rotationSpeed;

    public float currentSpeed;

    public float travelAngle;
    public float facingAngle;

    public Quaternion QuatCurrent;

    public float angleDif;

    public Vector3 colVol;

    public Vector3 currentVel;

    float controlCounter = 0;

    public bool discreteSlip;

    Rigidbody rb;

    public float boostCoolDown = 0;
    public float boostInterval = 2f;

    public float cameraDir;


    [Networked]
  public Vector3 MovementDirection { get; set; }

  public bool TransformLocal = false;

  [DrawIf(nameof(ShowSpeed), Hide = true)]
  public float Speed = 6f;

  bool ShowSpeed => this && !TryGetComponent<NetworkCharacterControllerPrototype>(out _);

  public void Awake() {
    CacheComponents();
  }

  public override void Spawned() {
    CacheComponents();
  }

  private void CacheComponents() {
    if (!_ncc) _ncc     = GetComponent<NetworkCharacterControllerPrototype>();
    if (!_nrb) _nrb     = GetComponent<NetworkRigidbody>();
    if (!_nrb2d) _nrb2d = GetComponent<NetworkRigidbody2D>();
    if (!_nt) _nt       = GetComponent<NetworkTransform>();
  }


    void goForward()
    {
        if (currentSpeed == 0)
        {
            travelAngle = facingAngle;
        }
        if (currentSpeed < topSpeed)
        {
            currentSpeed += acceleration * Time.deltaTime;
        }
        else
        {
            currentSpeed -= acceleration * Time.deltaTime;
        }


        float slip;
        Vector3 faceDirVector = Quaternion.Euler(0, facingAngle, 0) * Vector3.forward;
        //Vector3 faceDirVector = Quaternion.Euler(0, _nrb.Rigidbody.rotation.y, 0) * Vector3.forward;


        Vector3 travelDirVector = Quaternion.Euler(0, travelAngle, 0) * Vector3.forward;
        //Vector3 travelDirVector = _nrb.Rigidbody.velocity;
        slip = Vector3.SignedAngle(faceDirVector, travelDirVector, Vector3.up);
        if (slip < 0)
        {
            travelAngle += slipAmount;// * Time.deltaTime;
        }
        else if (slip > 0)
        {
            travelAngle -= slipAmount;// * Time.deltaTime;
        }
        Debug.Log(slip);

    }

    void doRotation(char dir)
    {
        /*
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            transform.Rotate(new Vector3(0, 1, 0), -rotationSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
           transform.Rotate(new Vector3(0, 1, 0), rotationSpeed * Time.deltaTime);
        }*/


        if (dir == 'L')
        {
            //transform.Rotate(new Vector3(0, 1, 0), -rotationSpeed * Time.deltaTime);
            _nrb.Rigidbody.MoveRotation(_nrb.Rigidbody.rotation * Quaternion.Euler(0, -rotationSpeed, 0));
            
        }

        if (dir == 'R')
        {
            //transform.Rotate(new Vector3(0, 1, 0), rotationSpeed * Time.deltaTime);
            _nrb.Rigidbody.MoveRotation(_nrb.Rigidbody.rotation * Quaternion.Euler(0, rotationSpeed, 0));
        }

        if(dir == 'U')
        {
           // _nrb.Rigidbody.rotation
        }

    }

    void coast()
    {
        if (currentSpeed > 0)
        {
            currentSpeed -= acceleration * 0.5f * Time.deltaTime;
        }
        float slip;
        Vector3 faceDirVector = Quaternion.Euler(0, facingAngle, 0) * Vector3.forward;
        //Vector3 faceDirVector = Quaternion.Euler(0, _nrb.Rigidbody.rotation.y, 0) * Vector3.forward;


        Vector3 travelDirVector = Quaternion.Euler(0, travelAngle, 0) * Vector3.forward;
        //Vector3 travelDirVector = _nrb.Rigidbody.velocity;
        slip = Vector3.SignedAngle(faceDirVector, travelDirVector, Vector3.up);
        if (slip < 0)
        {
            travelAngle += slipAmount * 3;
        }
        else if (slip > 0)
        {
            travelAngle -= slipAmount * 3;
        }
        Debug.Log(slip);
    }


    public override void FixedUpdateNetwork() {
    if (Runner.Config.PhysicsEngine == NetworkProjectConfig.PhysicsEngines.None) {
      return;
    }




        facingAngle = _nrb.Rigidbody.transform.rotation.eulerAngles.y;


        if (GetInput(out NetworkInputPrototype data))
        {
            if (data.IsDown(NetworkInputPrototype.BUTTON_FORWARD))
            {
                goForward();
            }
            else
            {
                coast();
            }
            /*data.direction.Normalize();
            _cc.Move(5 * data.direction * Runner.DeltaTime);*/

            if (data.IsDown(NetworkInputPrototype.BUTTON_LEFT))
            {
                doRotation('L');
            }
            if (data.IsDown(NetworkInputPrototype.BUTTON_RIGHT))
            {
                doRotation('R');
            }


        }

        //doRotation();
        Vector3 direction;
        //angleDif = Mathf.Abs(travelAngle - facingAngle);
        float slip;
        Vector3 faceDirVector = Quaternion.Euler(0, facingAngle, 0) * Vector3.forward;
        Vector3 travelDirVector = Quaternion.Euler(0, travelAngle, 0) * Vector3.forward;
        slip = Vector3.SignedAngle(faceDirVector, travelDirVector, Vector3.up);
        slip = Mathf.Abs(slip);
        if (slip > slipThreshold)
        {
            //dirVector = Quaternion.Euler(0, -travelAngle, 0) * Vector3.left;
            direction = Quaternion.Euler(0, travelAngle, 0) * Vector3.forward;
            direction = direction.normalized;
            Debug.Log("Slipping");
        }
        else
        {
            //dirVector = Quaternion.Euler(0, -facingAngle, 0) * Vector3.left;
            if (discreteSlip)
            {
                direction = Quaternion.Euler(0, facingAngle, 0) * Vector3.forward;
            }
            else
            {
                direction = Quaternion.Euler(0, travelAngle, 0) * Vector3.forward;
            }
            direction = direction.normalized;
            Debug.Log("Gripping");
        }
        currentVel = Vector3.Lerp(currentVel, direction * currentSpeed, 0.5f);

        currentVel += colVol;
        

        if (_ncc) {
      _ncc.Move(direction);
            //Debug.Log("Moving CC");
    } else if (_nrb && !_nrb.Rigidbody.isKinematic) {
            //Debug.Log("Moving RB");
      _nrb.Rigidbody.velocity = (currentVel);
    } else if (_nrb2d && !_nrb2d.Rigidbody.isKinematic) {
      Vector2 direction2d = new Vector2(direction.x, direction.y + direction.z);
      _nrb2d.Rigidbody.AddForce(direction2d * Speed);
    } else {
      transform.position += (direction * Speed * Runner.DeltaTime);
    }
  }
}