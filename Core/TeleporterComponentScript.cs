// Copyright © bl4ck & XDev, 2022-2024
using System.Collections.Generic;
using UnityEngine;
using Physics = goddamnit.Physics;

public class TeleporterComponentScript : MonoBehaviour
{
    public int RecieverID;
    public bool WorkAsReciever, RecieverTPBack, KeepYVelocity, CanTeleport = true, teleportObjects;

    public Material blue, yellow;

    public GameObject TeleporterBlue, TeleporterYellow;
    public Vector2 force;
    public LineRenderer lr;

    public bool canEnableLineRenderer;

    public GameObject[] childParticles;

    GameObject LocateFirst(bool beAReciever)
    {
        foreach (var levelObject in MainEditorComponent.Instance.levelObjects)
        {
            if (beAReciever)
            {
                var teleporterComponent = levelObject.GetComponent<TeleporterComponentScript>();
                if (teleporterComponent != null && teleporterComponent.RecieverID == RecieverID && teleporterComponent.WorkAsReciever)
                    return levelObject;
            }
            else
            {
                var teleporterComponent = levelObject.GetComponent<TeleporterComponentScript>();
                if (teleporterComponent != null && teleporterComponent.RecieverID == RecieverID && !teleporterComponent.WorkAsReciever)
                    return levelObject;
            }
        }
        return null;
    }

    private void Start()
    {
        if (WorkAsReciever)
            GetComponent<ObjectComponent>().savedMaterial[1] = blue;
        GetComponent<ObjectComponent>().ResetMaterials();
    }

    public void Update()
    {
        foreach (GameObject particle in childParticles)
        {
            particle.transform.localScale = transform.localScale;
        }
        
        if (WorkAsReciever)
        {
            GetComponent<ObjectComponent>().savedMaterial[1] = blue;
            TeleporterBlue.SetActive(true);
            TeleporterYellow.SetActive(false);

            lr.gameObject.SetActive(true);
            GameObject linkedPortal = MainEditorComponent.Instance.blackHoles.Find(x => x.GetComponent<TeleporterComponentScript>().RecieverID == RecieverID && x.GetComponent<TeleporterComponentScript>().WorkAsReciever == false);
            if (linkedPortal != null && canEnableLineRenderer)
            {
                lr.positionCount = 2;
                lr.SetPosition(0, new Vector3(transform.position.x, transform.position.y, 0));
                lr.SetPosition(1, new Vector3(linkedPortal.transform.position.x, linkedPortal.transform.position.y, 0));
            }
            else lr.gameObject.SetActive(false);

        }
        else
        {
            lr.gameObject.SetActive(true);
            GameObject linkedPortal = MainEditorComponent.Instance.blackHoles.Find(x => x.GetComponent<TeleporterComponentScript>().RecieverID == RecieverID && x.GetComponent<TeleporterComponentScript>().WorkAsReciever);
            if (linkedPortal != null && canEnableLineRenderer)
            {
                lr.positionCount = 2;
                lr.SetPosition(0, new Vector3(transform.position.x, transform.position.y, 0));
                lr.SetPosition(1, new Vector3(linkedPortal.transform.position.x, linkedPortal.transform.position.y, 0));
            }
            else lr.gameObject.SetActive(false);
            
            GetComponent<ObjectComponent>().savedMaterial[1] = yellow;
            TeleporterBlue.SetActive(false);
            TeleporterYellow.SetActive(true);
        }
        if (MainEditorComponent.Instance.editorControls.pstate == EditorControls.PlayState.play) lr.gameObject.SetActive(false);
    }

    public List<GameObject> teleportedObjects;

    private void OnTriggerEnter(Collider collision)
    {
        
        if (!teleportObjects)
        {
            if (collision.gameObject.CompareTag("Player") && !teleportedObjects.Contains(collision.gameObject))
            {
                Vector3 newPosition;

                if (RecieverTPBack && WorkAsReciever)
                {
                    GameObject loc = LocateFirst(false);
                    newPosition = loc.transform.position;
                    loc.GetComponent<TeleporterComponentScript>().CanTeleport = false;
                }
                else if (!RecieverTPBack && WorkAsReciever) return;
                else
                {
                    GameObject loc = LocateFirst(true);
                    newPosition = loc.transform.position;
                    loc.GetComponent<TeleporterComponentScript>().CanTeleport = false;
                }
                Rigidbody playerRigidbody = collision.gameObject.GetComponent<Rigidbody>();
                if (!KeepYVelocity)
                {

                    if (playerRigidbody != null)
                    {
                        Vector3 velocity = playerRigidbody.velocity;
                        velocity.y = 0f;
                        playerRigidbody.velocity = velocity;
                    }
                }
                playerRigidbody.AddForce(force, ForceMode.Impulse);


                collision.transform.position = newPosition;
            }
        }
        else
        {
            if (!teleportedObjects.Contains(collision.gameObject))
            {
                if (!collision.gameObject.TryGetComponent(out PlayerMovement pm))
                {
                    if (collision.gameObject.TryGetComponent(out Physics phy))
                    {
                        if (!phy.isEnabled)
                            return;
                    }

                    else if (!collision.gameObject.TryGetComponent(out Bullet b))
                        return;
                }
                
                Vector3 newPosition;
                
                if (RecieverTPBack && WorkAsReciever)
                {
                    print("t");
                    GameObject loc = LocateFirst(false);
                    newPosition = loc.transform.position;
                    loc.GetComponent<TeleporterComponentScript>().teleportedObjects.Add(collision.gameObject);
                }
                else if (!RecieverTPBack && WorkAsReciever) return;
                else
                {
                    GameObject loc = LocateFirst(true);
                    newPosition = loc.transform.position;
                    loc.GetComponent<TeleporterComponentScript>().teleportedObjects.Add(collision.gameObject);
                }
                Rigidbody playerRigidbody = collision.gameObject.GetComponent<Rigidbody>();
                if (!KeepYVelocity)
                {
                    if (playerRigidbody != null)
                    {
                        
                        Vector3 velocity = playerRigidbody.velocity;
                        velocity.y = 0f;
                        playerRigidbody.velocity = velocity;
                    }
                }
                if (playerRigidbody != null)
                playerRigidbody.AddForce(force, ForceMode.Impulse);

                collision.transform.position = newPosition;
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        teleportedObjects.Remove(collision.gameObject);
    }
}
