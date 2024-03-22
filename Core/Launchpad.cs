// Copyright © bl4ck & XDev, 2022-2024
using Steamworks.Data;
using UnityEngine;
using UnityEngine.Playables;
using CameraShaker = EZCameraShake.CameraShaker;

public class Launchpad : MonoBehaviour
{
    public float x, y;
    public GameObject child;
    private ParticleSystem.ShapeModule ps;
    [Space]
    public bool finale;
    public Material red, gold;
    public PlayableDirector timeline;

    public bool a;
    public bool finaleOpened;

    private void Start()
    {
        if (finale) timeline.Stop();
        
        ps = GetComponentInChildren<ParticleSystem>().shape;
    }
    private void Update()
    {
        if (finale) return; ps.scale = new Vector3(1, child.transform.localScale.y * transform.localScale.x, 1);
    }
    public GameObject boss;
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || (collision.gameObject.TryGetComponent(out goddamnit.Physics phy) && phy.isEnabled))
        {
            if (finale && !GetComponent<Portal>().isOpen) return;
            if (finale)
            {
                if (!finaleOpened) collision.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(x, y));
            }
            if (finale && GetComponent<Portal>().isOpen && !finaleOpened)
            {
                CameraShaker.Instance.ShakeOnce(3f, 1.5f, 0.2f, 0.2f);
                collision.gameObject.GetComponent<PlayerMovement>().enabled = false;
                boss.SetActive(true);
                timeline.Play();
                finaleOpened = true;
                PauseMenu.Instance.skip.SetActive(false);
                PauseMenu.Instance.exploration.SetActive(false);
                if (PlayerPrefs.GetInt("Exploration") == 0) PlayerPrefs.SetInt("Beat", 1);
                if (MainEditorComponent.Instance.player.GetComponent<Die>().deathCounter == 1)
                {
                    try
                    {
                        var addict = new Achievement("NATURAL_TALENT");
                        if (!addict.State && PlayerPrefs.GetInt("Exploration") == 0) addict.Trigger();
                    }
                    catch {}
                }
                GameObject.FindWithTag("Music").GetComponent<Music>().Pause();
            }
           if (!finale) collision.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(x, y));
            if (!a && collision.gameObject.TryGetComponent(out Die d)) d.launch.Play();

        }
    }
    //IEnumerator Cutscene(GameObject player)
    //{
    //    yield return new WaitForSeconds(0.2f);
    //    player.gameObject.GetComponent<Die>().ec.cine.GetComponent<CinemachineVirtualCamera>().Follow = transform.GetChild(1);
    //    yield return new WaitForSeconds(0.6f);
    //    player.gameObject.GetComponent<Die>().ec.mainEditorComponent.portalout.GetComponent<MeshRenderer>().material = red;
    //    player.gameObject.GetComponent<Die>().ec.mainEditorComponent.portalout.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material = red;
    //    yield return new WaitForSeconds(0.1f);
    //    player.gameObject.GetComponent<Die>().ec.mainEditorComponent.portalout.GetComponent<MeshRenderer>().material = gold;
    //    player.gameObject.GetComponent<Die>().ec.mainEditorComponent.portalout.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material = gold;
    //    yield return new WaitForSeconds(0.4f);
    //    player.gameObject.GetComponent<Die>().ec.mainEditorComponent.portalout.GetComponent<MeshRenderer>().material = red;
    //    player.gameObject.GetComponent<Die>().ec.mainEditorComponent.portalout.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material = red;
    //    yield return new WaitForSeconds(0.1f);
    //    player.gameObject.GetComponent<Die>().ec.mainEditorComponent.portalout.GetComponent<MeshRenderer>().material = gold;
    //    player.gameObject.GetComponent<Die>().ec.mainEditorComponent.portalout.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material = gold;
    //    yield return new WaitForSeconds(0.2f);
    //    player.gameObject.GetComponent<Die>().ec.mainEditorComponent.portalout.GetComponent<MeshRenderer>().material = red;
    //    player.gameObject.GetComponent<Die>().ec.mainEditorComponent.portalout.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material = red;
    //    yield return new WaitForSeconds(0.1f);
    //    player.gameObject.GetComponent<Die>().ec.mainEditorComponent.portalout.GetComponent<MeshRenderer>().material = gold;
    //    player.gameObject.GetComponent<Die>().ec.mainEditorComponent.portalout.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material = gold;
    //    yield return new WaitForSeconds(0.1f);
    //    player.gameObject.GetComponent<Die>().ec.mainEditorComponent.portalout.GetComponent<MeshRenderer>().material = red;
    //    player.gameObject.GetComponent<Die>().ec.mainEditorComponent.portalout.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material = red;
    //}
}
