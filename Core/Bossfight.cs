// Copyright © bl4ck & XDev, 2022-2024
using Cinemachine;
using DG.Tweening;
using EZCameraShake;
using Steamworks.Data;
using System;
using System.Collections;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Color = UnityEngine.Color;
using Image = UnityEngine.UI.Image;

public class Bossfight : MonoBehaviour
{

    public enum Attackpatterns
    {
        Bounce,
        Shoot,
        Laser,
        SplitTarget,
        DeadlyFloor,
        LevelSpawn,
        WorldRotate,
        FlipAroundScreen,
    };

    public enum PlayerAttackPatterns
    {
        Turret,
    };

    public Transform explosion;

    public static Bossfight Instance;
    public bool cutscene = true;

    public LayerMask destroyable;
    public Vector2 force;

    public Attackpatterns attackPattern;
    public PlayerAttackPatterns playerAttackPattern;
    public Rigidbody rigid;
    public PhysicMaterial bounce;
    public Camera cam;


    public AudioSource crashSFX, shootSFX; // for big turret
    
    // Resets to zero after a player attack finishes.
    public int _attackCount;

    private void Start()
    {
        Instance = this;
        cutscene = true;
        health = 65;
        _attackCount = -1;
        MainEditorComponent.Instance.editorControls.sta = true;
        rigid = GetComponent<Rigidbody>();
        
    }

    #region Cutscene
    private void OnCollisionEnter(Collision other)
    {
        //if (other.gameObject.layer == 3)
        //{
        if ((!startAttacking && cutscene || attackPattern == Attackpatterns.LevelSpawn) && !other.gameObject.name.Contains("Wall"))
        {
            other.gameObject.GetComponent<Collider>().enabled = false;
            other.gameObject.AddComponent<Rigidbody>();
            Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
            if (other.gameObject.transform.position.y > transform.position.y) rb.AddForce(new Vector3(force.x, force.y, 0), ForceMode.Impulse);
            else rb.AddForce(new Vector3(force.x, -force.y, 0), ForceMode.Impulse);
            Shake(1);
            //rb.angularVelocity = force * Vector3.forward * 10;
            //}
        }
        else
        {
            if (attackPattern == Attackpatterns.Bounce)
            {
                Shake(1);
                if (other.gameObject.CompareTag("Player")) die.PDie();
            }
        }
        
        

    }

    public bool look;
    private bool startAttacking;
    private bool stopAttacking;



    [Space]
    [Header("DO NOT TOUCH PLEASE OK THANKS")]
    public GameObject face;
    public Vector3 offset;
    public float asd;


    public AxisConstraint axis;
    public Vector3 ahs;

    private bool fr;

    public void Shake(float magnitude)
    {
        CameraShaker.Instance.ShakeOnce(magnitude, 1, 0.1f, 0.1f);
    }

    public void StartExplosion() => StartCoroutine(Explosion());

    IEnumerator Explosion()
    {
        cutscene = false;
        ObjectComponent[] a = UnityEngine.Object.FindObjectsOfType<ObjectComponent>();

        for (int i = 0; i < a.Length; i++)
        {
            if (a[i].TryGetComponent(out Collider b)) b.enabled = false;
            a[i].AddComponent<Rigidbody>();
            a[i].GetComponent<Rigidbody>().useGravity = true;
            a[i].GetComponent<Rigidbody>().AddForce(Vector3.down * 5);
        }
        yield return new WaitForSeconds(1);
        player.transform.position = new Vector3(0, 0, 200);
        for (int i = 0; i < a.Length; i++)
        {
            a[i].GetComponent<Rigidbody>().AddForce(a[i].transform.position * new Vector2(Mathf.Abs(force.x), force.y) / 3, ForceMode.Impulse);
            a[i].GetComponent<Rigidbody>().angularVelocity = force * Vector3.forward * 10;
        }
        yield return new WaitForSeconds(3);
    }
    public CinemachineVirtualCamera higherFOV;
    public void Destroy() => UnityEngine.Object.FindObjectsOfType<ObjectComponent>().ToList().ForEach(o => Destroy(o.gameObject));
    
    public void Unfreeze()
    {
        die.deathCounter = 1;
        if (Music.instance != null)
        {
            Music.instance.music[0] = bossTrack;
            Music.instance.source.clip = bossTrack;
            Music.instance.Resume();
        }


        
        die.finale = true;
        player.GetComponent<PlayerMovement>().enabled = true;
        player.transform.position = new Vector3(0, -8, 0);
        higherFOV.Priority = 50;
        bossCanvas.SetActive(true);
        StartCoroutine(Focus());
        _waitForPattern = StartCoroutine(waitForPattern());
        look = true;
        startAttacking = true;
        gameObject.layer = 13;
    }

    IEnumerator Focus()
    {
        yield return new WaitForSeconds(1f);
        higherFOV.Follow = center.transform;
        higherFOV.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = 50;
        yield return new WaitForSeconds(0.8f);
        higherFOV.Follow = null;
    }
    [Space]
    public GameObject bossCanvas;
    public GameObject player;
    public GameObject center;
    public AudioSource src;
    public AudioClip bossTrack;
    public Die die;

    public Slider healthbar;
    public Image healthbarBackground;
    public float health;
    public int attackDealt;

    public GameObject missileWallLeft, missileWallRight;
    public GameObject levelSpawn;
    private Transform _levelSpawn;
    public CanvasGroup c;
    public void TakeDamage(int dmg)
    {
        transform.localScale = new Vector3(3.4f, 3.4f, 1);
        transform.DOScale(new Vector3(4f, 4f, 1), 0.3f);
        attackDealt += dmg;
        health -= dmg;
        DOTween.To(() => healthbar.value, x => healthbar.value = x, health, 0.4f);
        healthbarBackground.color = new Color(1f, 0.3f, 0.3f);
        DOTween.To(() => healthbarBackground.color, x => healthbarBackground.color = x, Color.white, 0.4f);
        
        damage.Play();
        if (health <= 0)
        {
            fr = true;
            StartCoroutine(Die());
        }
        else Shake(1);
    }

    public AudioSource damage;
    public AudioSource expl;
    IEnumerator Die()
    {
        yield return new WaitForSeconds(1);
        CameraShaker.Instance.ShakeOnce(5, 3, 0.2f, 0.2f);
        try { StopAttacks(); } catch { }
        StopCoroutine(_waitForPattern);
        transform.DOScale(0, 0.1f);
        Instantiate(explosion, transform.position, Quaternion.identity);
        c.gameObject.SetActive(true);
        Scroll.toggleBefore = true;
        DOTween.To(() => c.alpha, x => c.alpha = x, 1, 5f);
        DOTween.To(() => Music.instance.source.volume, x => Music.instance.source.volume = x, 0, 5f);
        DOTween.To(() => higherFOV.m_Lens.FieldOfView, x => higherFOV.m_Lens.FieldOfView = x, 27, 5f).OnComplete(() =>
        {
            SceneManager.LoadSceneAsync("Full Credits");
        });
        higherFOV.Follow = center.transform;
        expl.Play();
        yield return new WaitForSeconds(0.1f);
        expl.Play();
        yield return new WaitForSeconds(0.3f);
        expl.Play();
    }
    #endregion
    
    #region Bossfight
    public Coroutine _waitForPattern;

    public IEnumerator waitForPattern()
    {
        attackDealt = 0;
        yield return new WaitForSeconds(2f);
        if (_attackCount == roundsBetweenPlayerAttack)
        {
            _attackCount = 0;
        }
        else _attackCount++;
        ChangeAttackPattern(_attackCount == roundsBetweenPlayerAttack);
        yield return new WaitForSeconds(UnityEngine.Random.Range(30, 45));
        StopAttacks();
        _waitForPattern = StartCoroutine(waitForPattern());
    }

    public void DoNothing()
    {
        StopCoroutine(_waitForPattern);
    }
    public void DoSomething()
    {
        try { StopCoroutine(_waitForPattern);} catch { print("_waitForPattern doesn't exist");}
        _waitForPattern = StartCoroutine(waitForPattern());
    }
    void ChangeAttackPattern(bool pl)
    {
        UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
        if (!pl)
            attackPattern = (Attackpatterns)UnityEngine.Random.Range(0, Enum.GetNames(typeof(Attackpatterns)).Length); 

        else playerAttackPattern = (PlayerAttackPatterns)UnityEngine.Random.Range(0, Enum.GetNames(typeof(PlayerAttackPatterns)).Length);

        StartAttacking(pl);
    }


    public GameObject spikeFloor;
    private GameObject _spikeFloor;
    public Transform missile;
    public GameObject[] turrets;
    public Transform walls;
    public Transform turnAround;
    private Transform _turnAround;
    private Transform _turnAround2;
    public Transform wallAnchor;
    public Transform laser;
    public Transform turret;

    private Coroutine _flipAttack;

    public bool moving;

    IEnumerator FlipAttack()
    {
        _turnAround = Instantiate(turnAround);
        higherFOV.transform.DORotate(new Vector3(0, 0, 180), 0.5f);
        Physics.gravity = Physics.gravity = new Vector3(0, 15, 0);
        player.GetComponent<PlayerMovement>().runSpeed = -player.GetComponent<PlayerMovement>().runSpeed;
        player.GetComponent<CharacterController2D>().m_JumpForce = -player.GetComponent<CharacterController2D>().m_JumpForce;
        yield return new WaitForSeconds(5);
        _turnAround2 = Instantiate(turnAround, new Vector3(turnAround.transform.position.x, -15.3f, 0), Quaternion.Euler(0, 0, 180));
        _turnAround2.GetComponent<Moving>().from = _turnAround2.transform.position;
        _turnAround2.GetComponent<Moving>().to.y = _turnAround2.transform.position.y;
        _turnAround2.GetComponent<Moving>().to.x += 2;
        higherFOV.transform.DORotate(new Vector3(0, 0, 0), 0.5f);
        Physics.gravity = Physics.gravity = new Vector3(0, -10, 0);
        player.GetComponent<PlayerMovement>().runSpeed = -player.GetComponent<PlayerMovement>().runSpeed;
        player.GetComponent<CharacterController2D>().m_JumpForce = -player.GetComponent<CharacterController2D>().m_JumpForce;
        yield return new WaitForSeconds(5);
        higherFOV.transform.DORotate(new Vector3(0, 0, 180), 0.5f);
        Physics.gravity = Physics.gravity = new Vector3(0, 15, 0);
        player.GetComponent<PlayerMovement>().runSpeed = -player.GetComponent<PlayerMovement>().runSpeed;
        player.GetComponent<CharacterController2D>().m_JumpForce = -player.GetComponent<CharacterController2D>().m_JumpForce;
        yield return new WaitForSeconds(4);

        StopAttacks();
        yield return new WaitForSeconds(1.5f);
        DoSomething();
    }
    public GameObject cellA, cellB, full, lace;
    void StartAttacking(bool playerAttack = false)
    {
        startAttacking = true;
        rigid.constraints = RigidbodyConstraints.FreezePosition;
        if (!playerAttack)
        {
            switch (attackPattern)
            {
                case Attackpatterns.Bounce:
                    transform.DORotate(Vector3.zero, 0.2f);
                    gameObject.layer = 16;
                    look = false;
                    rigid.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
                    GetComponent<Collider>().sharedMaterial = bounce;
                    _spikeFloor = Instantiate(spikeFloor);
                    _spikeFloor.transform.DOMoveX(0, 3f);
                    rigid.velocity = new Vector3(20, 20, 0);
                    foreach (GameObject turret in turrets)
                    {
                        turret.SetActive(true);
                        turret.transform.DOScale(1, 0.5f);
                        turret.GetComponent<Turret>().freeze = false;
                    }
                    break;

                case Attackpatterns.Shoot:
                    walls.gameObject.SetActive(true);
                    _spawnMissiles = StartCoroutine(SpawnMissiles());
                    higherFOV.Follow = player.transform;
                    walls.DOScale(1, 0.5f);
                    DOTween.To(() => higherFOV.m_Lens.FieldOfView, x => higherFOV.m_Lens.FieldOfView = x, 27, 0.6f);
                    break;

                case Attackpatterns.Laser:
                    DOTween.To(() => higherFOV.m_Lens.FieldOfView, x => higherFOV.m_Lens.FieldOfView = x, 27, 0.6f);
                    higherFOV.Follow = player.transform;
                    topWall.transform.DOLocalMoveY(70, 1f);
                    _laserFloor = StartCoroutine(LaserFloor());
                    break;

                case Attackpatterns.SplitTarget:
                    face.transform.DOScale(0, 0.2f);
                    face.transform.DOLocalMoveY(1, 0.2f);
                    freeze = true;
                    GetComponent<BoxCollider>().enabled = false;
                    _splitTarget = StartCoroutine(SplitTarget());
                    break;

                case Attackpatterns.DeadlyFloor:
                    _deadlyFloor = StartCoroutine(DeadlyFloor());
                    break;

                case Attackpatterns.LevelSpawn:
                    transform.DOMoveX(-10, 0.5f).OnComplete(() =>
                    {
                        lace.SetActive(true);
                        transform.DOScale(new Vector3(8, 8, 1), 0.5f);//.OnComplete(() => lace.transform.DOScale(Vector3.one, 0.5f));
                        transform.DOMoveZ(-3, 0.5f);
                        rightWall.transform.DOLocalMoveX(180, 0.5f);
                        higherFOV.Follow = player.transform;
                        moving = true;
                        look = false;
                        transform.DORotate(new Vector3(0, -30, 0), 0.5f);
                        higherFOV.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = 200000;
                        DOTween.To(() => higherFOV.m_Lens.FieldOfView, x => higherFOV.m_Lens.FieldOfView = x, 48, 0.5f);
                        DOTween.To(() => higherFOV.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX, x => higherFOV.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX = x, 0.4f, 0.5f);
                        StartCoroutine(LevelSpawn());
                        
                    });
                    //lace.transform.localScale = Vector3.zero;

                    break;

                case Attackpatterns.WorldRotate:
                    rotateAnchor = true;
                    leftWall.transform.DOMoveX(-53f, 0.5f);
                    player.GetComponent<Rigidbody>().AddForce(Vector3.up * 10, ForceMode.Impulse);
                    foreach (Transform wall in wallAnchor.transform)
                    {
                        wall.gameObject.layer = 3;
                        foreach (Transform portal in wall.transform) portal.gameObject.SetActive(false);
                    }
                    rightWall.transform.DOMoveX(53f, 0.5f).OnComplete(() =>
                    {
                        spikeRotate.SetActive(true);
                        foreach (Transform spike in spikeRotate.transform) spike.gameObject.SetActive(true);
                        spikeRotate.transform.DOScale(1, 0.5f);
                    });
                    DOTween.To(() => higherFOV.m_Lens.FieldOfView, x => higherFOV.m_Lens.FieldOfView = x, 48, 0.3f);
                    
                    break;

                case Attackpatterns.FlipAroundScreen:
                    _flipAttack = StartCoroutine(FlipAttack());
                    break;
            }
        }

        else
        {
            switch (playerAttackPattern)
            {
                case PlayerAttackPatterns.Turret:
                    
                    _turret = Instantiate(turret);
                    _turret.transform.localPosition += new Vector3(0, 50, 0);
                    float pos = _turret.transform.localPosition.y;
                    _turret.DOLocalMoveY(pos - 50, 0.1f);
                    transform.DOMoveX(10, 1f).OnComplete(Move);
                    print("a");
                    
                    break;
            }
        }

    }
    public TMP_Text bossAttemptCount;
    public void StopAttacks()
    {

        StartCoroutine(Focus());
        look = true;
        switch (attackPattern)
        {
            case Attackpatterns.Bounce:
                gameObject.layer = 13;
                GetComponent<Collider>().material = null;
                transform.DOMove(center.transform.position, 1f);
                rigid.velocity = Vector3.zero;
                Destroy(_spikeFloor);
                foreach (GameObject turret in turrets)
                {
                    turret.SetActive(false);
                    turret.transform.localScale = Vector3.zero;
                    turret.GetComponent<Turret>().freeze = true;
                }
                break;
            case Attackpatterns.Shoot:
                walls.DOScale(new Vector3(2, 2, 1), 0.5f);
                StopCoroutine(_spawnMissiles);
                missileWallLeft.transform.DOKill();
                missileWallRight.transform.DOKill();
                missileWallLeft.transform.DOLocalMoveX(-21.75f, 0.2f);
                missileWallRight.transform.DOLocalMoveX(21.75f, 0.2f).OnComplete(() => walls.gameObject.SetActive(false));
                DOTween.To(() => higherFOV.m_Lens.FieldOfView, x => higherFOV.m_Lens.FieldOfView = x, 42, 0.6f);
                break;
            case Attackpatterns.FlipAroundScreen:
                StopCoroutine(_flipAttack);
                Physics.gravity = new Vector3(0, -10, 0);
                try
                {
                    Destroy(_turnAround.gameObject);
                }
                catch { }
                try
                {
                    Destroy(_turnAround2.gameObject);
                }
                catch {}
                DoSomething();
                higherFOV.transform.DORotate(new Vector3(0, 0, 0), 0.5f);
                player.GetComponent<PlayerMovement>().runSpeed = Mathf.Abs(player.GetComponent<PlayerMovement>().runSpeed);
                player.GetComponent<CharacterController2D>().m_JumpForce = Mathf.Abs(player.GetComponent<CharacterController2D>().m_JumpForce);
                break;
            case Attackpatterns.DeadlyFloor:
                StopCoroutine(_deadlyFloor);
                Destroy(_platforms);
                break;
            case Attackpatterns.WorldRotate:
                rotateAnchor = false;
                wallAnchor.DORotate(Vector3.zero, 0.6f);
                leftWall.transform.DOLocalMoveX(-60f, 0.2f);
                rightWall.transform.DOLocalMoveX(60f, 0.5f);
                spikeRotate.SetActive(false);
                foreach (Transform wall in wallAnchor)
                {
                    if (wall.name != "Bottom Wall") wall.gameObject.layer = 0;
                    foreach (Transform portal in wall.transform) portal.gameObject.SetActive(true);

                }
                
                DOTween.To(() => higherFOV.m_Lens.FieldOfView, x => higherFOV.m_Lens.FieldOfView = x, 42, 0.3f);
                topWall.transform.DOLocalMoveY(44.5f, 0.5f);
                break;
            case Attackpatterns.Laser:
                StartCoroutine(StopLaser());
                break;
            case Attackpatterns.LevelSpawn:
                moving = false;
                lace.SetActive(false);
                DoSomething();
                DOTween.To(() => higherFOV.m_Lens.FieldOfView, x => higherFOV.m_Lens.FieldOfView = x, 42, 0.5f);
                DOTween.To(() => higherFOV.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX, x => higherFOV.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX = x, 0.5f, 0.5f);
                _levelSpawn.DOMoveY(-50, 0.2f);
                transform.DOMoveX(0, 0.2f);
                StartCoroutine(Focus());
                transform.DORotate(Vector3.zero, 0.2f);
                player.GetComponent<PlayerMovement>().enabled = false;
                player.GetComponent<Rigidbody>().AddForce(new Vector3(-500, 50, 0), ForceMode.Impulse);
                higherFOV.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = 0.5f;
                print("stop level spawn please");
                rightWall.transform.DOLocalMoveX(60, 1f).OnComplete(() =>
                {
                    transform.DOScale(new Vector3(4, 4, 1), 0.2f);
                    Destroy(_levelSpawn.gameObject);
                    player.GetComponent<PlayerMovement>().enabled = true;
                    
                });
                
                break;
            case Attackpatterns.SplitTarget:

                freeze = true;
                
                cellA.GetComponent<Split>().shoot = false;
                cellB.GetComponent<Split>().shoot = false;
                GetComponent<BoxCollider>().enabled = true;

                
                cellA.GetComponent<Split>().OnEnable();
                cellB.GetComponent<Split>().OnEnable();
                
                cellA.transform.DOLocalRotate(new Vector3(-90, 0, 0), 0.5f);
                cellB.transform.DOLocalRotate(new Vector3(-90, 0, 0), 0.5f);
                cellA.transform.DOLocalMove(new Vector3(-0.69f, -0.12f, 1), 0.5f).OnComplete(() =>
                {
                    full.GetComponent<Renderer>().enabled = true;
                    face.transform.DOScale(1, 0.2f);
                    face.transform.DOLocalMoveY(-1, 0.2f);
                    face.SetActive(true);
                    cellA.SetActive(false);
                    cellB.SetActive(false);
                });
                cellB.transform.DOLocalMove(new Vector3(0.46f, 0.05f, 0), 0.5f);
                break;
            
        }

        switch (playerAttackPattern)
        {
            case PlayerAttackPatterns.Turret:
                transform.DOKill();
                transform.DOMove(Vector3.zero, 0.5f);
                _turret.DOScale(0, 0.3f).OnComplete(() => Destroy(_turret.gameObject));
                break;
        }

    }
    public IEnumerator hjhj()
    {
        yield return new WaitForSeconds(3);
        StartCoroutine(Focus());
    }
    Transform _turret;
    IEnumerator LevelSpawn()
    {
        _levelSpawn = Instantiate(levelSpawn).transform;
        foreach (Transform obj in _levelSpawn)
        {
            obj.transform.position -= new Vector3(0, 50, 0);
        }
        foreach (Transform obj in _levelSpawn)
        {
            float y = obj.transform.position.y + 50;

            obj.DOLocalMoveY(y, 0.5f);
            yield return new WaitForSeconds(0.01f);
        }

    }

    public GameObject spikeRotate;
    public GameObject platforms;
    private GameObject _platforms;

    private Coroutine _laserFloor;
    public Transform _laser;
    IEnumerator LaserFloor()
    {
        _laser = Instantiate(laser);

        foreach (Transform obj in _laser)
        {
            if (obj.name == "Floor") continue;
            obj.transform.localPosition += new Vector3(0, 50, 0);
        }

        foreach (Transform obj in _laser)
        {
            if (obj.name == "Floor") continue;
            float y = obj.transform.position.y - 50;
            obj.DOLocalMoveY(y, 0.5f);
            yield return new WaitForSeconds(0.05f);
        }


    }

    public void Move()
    {
        print("b");
        transform.DOMoveY(7, .9f).OnComplete(() => transform.DOMoveY(-7, .9f).OnComplete(Move));
    }

    private Coroutine _deadlyFloor;
    IEnumerator DeadlyFloor()
    {
        _platforms = Instantiate(platforms);
        foreach (Transform platform in _platforms.transform)
        {
            if (platform.name.Contains("Turret"))
            {
                float b = platform.position.y;
                platform.position += Vector3.up * 30;
                platform.DOMoveY(b, 0.5f);
            }
            if (!platform.name.Contains("Platform")) continue;
            platform.position -= new Vector3(0, 30, 0);
        }
        foreach (Transform platform in _platforms.transform)
        {
            if (!platform.name.Contains("Platform")) continue;
            float y = platform.position.y;
            platform.DOMoveY(y + 30, 0.3f);
            yield return new WaitForSeconds(0.05f);
        }
        _platforms.transform.GetChild(6).DOMoveX(-5, 0.6f);
        _platforms.transform.GetChild(7).DOMoveX(5, 0.6f);
        yield return new WaitForSeconds(3);
        _platforms.transform.GetChild(6).DOMoveX(40, 0.6f);
        _platforms.transform.GetChild(7).DOMoveX(40, 0.6f);
        _platforms.transform.GetChild(8).DOMoveX(0, 0.6f);
        yield return new WaitForSeconds(1);
        int j = 0;
        for (int i = 0; i < 8; i++)
        {
            j = i;
            foreach (Transform platform in _platforms.transform)
            {
                if (!platform.name.Contains("Platform")) continue;
                j++;
                if (j % 2 == 0) continue;
                platform.GetComponent<Renderer>().material = transparent;
            }
            j = i;
            yield return new WaitForSeconds(0.15f);
            foreach (Transform platform in _platforms.transform)
            {
                if (!platform.name.Contains("Platform")) continue;
                j++;
                if (j % 2 == 0) continue;
                platform.GetComponent<Renderer>().material = originalPlatform;
            }
            j = i;
            yield return new WaitForSeconds(0.15f);
            foreach (Transform platform in _platforms.transform)
            {
                if (!platform.name.Contains("Platform")) continue;
                j++;
                if (j % 2 == 0) continue;
                platform.GetComponent<Renderer>().material = transparent;
            }
            j = i;
            yield return new WaitForSeconds(0.15f);
            foreach (Transform platform in _platforms.transform)
            {
                if (!platform.name.Contains("Platform")) continue;
                j++;
                if (j % 2 == 0) continue;
                platform.GetComponent<Renderer>().material = originalPlatform;
            }
            j = i;
            yield return new WaitForSeconds(0.15f);
            foreach (Transform platform in _platforms.transform)
            {
                if (!platform.name.Contains("Platform")) continue;
                j++;
                if (j % 2 == 0) continue;
                platform.GetComponent<Renderer>().material = transparent;
            }
            j = i;
            yield return new WaitForSeconds(0.15f);
            foreach (Transform platform in _platforms.transform)
            {
                if (!platform.name.Contains("Platform")) continue;
                j++;
                if (j % 2 == 0)
                {
                    continue;
                }
                platform.GetComponent<Renderer>().material = originalPlatform;
            }
            foreach (Transform platform in _platforms.transform)
            {
                if (!platform.name.Contains("Platform")) continue;
                j++;
                if (j % 2 == 0)
                {
                    platform.GetComponent<Renderer>().material = originalPlatform;
                    platform.GetComponent<Collider>().enabled = true;
                    continue;
                }
                platform.GetComponent<Renderer>().material = transparent;
                platform.GetComponent<Collider>().enabled = false;
            }
            yield return new WaitForSeconds(1.5f);
            j++;
        }
        StopCoroutine(_waitForPattern);

        _platforms.transform.GetChild(6).DOMoveX(-40, 0.6f);
        _platforms.transform.GetChild(7).DOMoveX(40, 0.6f);
        _platforms.transform.GetChild(8).DOMoveY(-20, 0.6f);
        foreach (Transform platform in _platforms.transform)
        {
            if (platform.name.Contains("Turret"))
            {
                float b = platform.position.y + 30;
                platform.DOMoveY(b, 0.5f);
            }
            if (!platform.name.Contains("Platform")) continue;
            float y = platform.position.y;
            platform.DOMoveY(y - 30, 0.3f);
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(1.5f);
        StopAttacks();
        _waitForPattern = StartCoroutine(waitForPattern());

    }

    public Material transparent;
    public Material originalPlatform;

    private Coroutine _splitTarget;
    IEnumerator SplitTarget()
    {
        cellA.SetActive(true);
        cellB.SetActive(true);
        full.GetComponent<Renderer>().enabled = false;

        yield return new WaitForSeconds(0.2f);
        face.SetActive(false);
        cellA.transform.DOLocalMove(new Vector3(-2, -1, 1), 0.5f);
        cellB.transform.DOLocalMove(new Vector3(2, 1, 1), 0.5f);
    }


    IEnumerator StopLaser()
    {
        
        StopCoroutine(_waitForPattern);
        _waitForPattern = StartCoroutine(waitForPattern());
        foreach (Transform obj in _laser)
        {
            if (obj.name == "Floor")
            {
                obj.GetComponent<Moving>().enabled = false;
            }
        }

        foreach (Transform obj in _laser)
        {
            float y = obj.transform.position.y - 75;
            obj.DOLocalMoveY(y, 0.5f);
            yield return new WaitForSeconds(0.05f);
        }
        topWall.transform.DOLocalMoveY(44.5f, 1f);
        DOTween.To(() => higherFOV.m_Lens.FieldOfView, x => higherFOV.m_Lens.FieldOfView = x, 42, 0.3f);
        player.GetComponent<Rigidbody>().AddForce(new Vector3(-200, -20, 0), ForceMode.Impulse);
        Shake(1);
        Destroy(_laser.gameObject);
    }

    Coroutine _spawnMissiles;
    IEnumerator SpawnMissiles()
    {
        Transform a = Instantiate(missile, transform.position + Vector3.forward, Quaternion.identity);
        yield return new WaitForSeconds(1f);
        a.GetComponent<Collider>().enabled = true;
        yield return new WaitForSeconds(5f);
        if (attackPattern == Attackpatterns.Shoot) _spawnMissiles = StartCoroutine(SpawnMissiles());
    }
    #endregion

    public bool freeze;
    public float moveSpeed;
    private void Update()
    {
        if (moving) transform.position += Time.deltaTime * Vector3.right * moveSpeed;
        if (look)
        {
            //print("d");
            Vector3 pos = -player.transform.position;
            transform.DOLookAt(pos + offset, 0.2f, axis, ahs);
        }
        if (attackDealt >= 10 && !fr)
        {
            StopCoroutine(_waitForPattern);
            _waitForPattern = StartCoroutine(waitForPattern());

            StopAttacks();
        }
        if (rotateAnchor) wallAnchor.transform.eulerAngles += rotateSpeed * Time.deltaTime;
        //if (rotateAnchor) wallAnchor.transform += rotateSpeed * Time.deltaTime;

    }
    public GameObject leftWall, rightWall, topWall;
    public bool rotateAnchor;
    public Vector3 rotateSpeed;
    [Space]
    [Header("Boss Fight")]
    public int roundsBetweenPlayerAttack;

    private void LateUpdate()
    {
        if (freeze) transform.position -= Vector3.forward * transform.position.z;
    }

}
