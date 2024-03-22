// Copyright © bl4ck & XDev, 2022-2024
// i fucking hate everything
using HSVPicker;
using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using Object = System.Object;

public class ObjectProperties : MonoBehaviour
{
    GameObject[] m;
    public EditorControls ec;
    public GameObject movePanel, spinPanel, colorPanel;
    public GameObject content;

    // Transform
    public Toggle spint, move, restart, loop, goBack, physics, lockX, lockY, lockZ, restartOnDeath, col_enable, g_col_enable, useTime;
    public TMP_InputField posX, posY, rotZ, scaleX, scaleY, spin, mass;
    public ColorPicker picker, g_picker;
    public UnityEngine.UI.Button plus;

    // Panels
    public GameObject turretPanel, buttonPanel, launchPadPanel, textPanel, tpPanel, chPanel;

    // Turret
    public Toggle t_followPlayer;
    public TMP_InputField t_bulletSpeed, t_offset, t_cooldown;

    // Launch Pad
    public TMP_InputField l_forceX, l_forceY;

    // Button
    public Toggle b_offOnRestart, b_turnBackOff, b_immidiatelyActive;

    // Text
    public TMP_InputField tx_contents, tx_fontSize;
    public Toggle tx_bold, tx_italic;

    //Teleporter
    public Toggle tp_WorkAsReciever, tp_RecieverTPBack, tp_ResetYVelocity, tp_teleportObjects;
    public TMP_InputField tp_RecieverID, tp_forceX, tp_forceY;
    public TMP_Text tp_t_workAsReceiver, tp_t_receiverID, tp_t_RecieverTpBack;
    public Color disabledColor;
    
    //Checkpoint
    public Toggle c_enabled, c_startPos;
    public TMP_InputField c_priority;

    public static ObjectProperties instance;

    private void Awake()
    {
        instance = this;
    }

    public void InitializeFields(GameObject[] g)
    {
        float[] xPos = new float[g.Length];
        float[] yPos = new float[g.Length];
        float[] xScale = new float[g.Length];
        float[] yScale = new float[g.Length];
        float[] rot = new float[g.Length];
        m = g;
        //if (g.Length > 1)
        //{
        //    movePanel.SetActive(false);
        //    spinPanel.SetActive(false);
        //}
        //else
        //{
        Moving mo = g[0].GetComponent<Moving>();
        //mo.points.ToList().ForEach(p => print(p));
        move.isOn = g.All(x => x.GetComponent<Moving>().isEnabled == mo.isEnabled) && mo.isEnabled;
        //wait.text = g.All(x => x.GetComponent<Moving>().wait == mo.wait) ? mo.wait.ToString() : "-";
        //speed.text = g.All(x => x.GetComponent<Moving>().speed == mo.speed) ? mo.speed.ToString() : "-";
        //toX.text = g.All(x => x.GetComponent<Moving>().to.x == mo.to.x) ? mo.to.x.ToString() : "-";
        //toY.text = g.All(x => x.GetComponent<Moving>().to.y == mo.to.y) ? mo.to.y.ToString() : "-";
        restart.isOn = g.All(x => x.GetComponent<Moving>().restartWhenDeath == mo.restartWhenDeath) && mo.restartWhenDeath;
        //stickable.isOn = g.All(x => x.GetComponent<Moving>().stickable == mo.stickable) && mo.stickable;
        loop.isOn = g.All(x => x.GetComponent<Moving>().loop == mo.loop) && mo.loop;
        goBack.isOn = g.All(x => x.GetComponent<Moving>().goBack == mo.goBack) && mo.goBack;
        useTime.isOn = g.All(x => x.GetComponent<Moving>().useTime == mo.useTime) && mo.useTime;

        if (m.Length == 1)
        {
            SetupCycle.instance.ClearCycles();
            SetupCycle.instance.FeedPoints(mo.points.ToArray());
        }


        goddamnit.Physics ph = g[0].GetComponent<goddamnit.Physics>();
        physics.isOn = g.All(x => x.GetComponent<goddamnit.Physics>().isEnabled == ph.isEnabled) && ph.isEnabled;
        mass.text = g.All(x => x.GetComponent<goddamnit.Physics>().mass == ph.mass) ? ph.mass.ToString() : "-";
        lockX.isOn = g.All(x => x.GetComponent<goddamnit.Physics>().lockX == ph.lockX) && ph.lockX;
        lockY.isOn = g.All(x => x.GetComponent<goddamnit.Physics>().lockY == ph.lockY) && ph.lockY;
        lockZ.isOn = g.All(x => x.GetComponent<goddamnit.Physics>().lockZ == ph.lockZ) && ph.lockZ;
        restartOnDeath.isOn = g.All(x => x.GetComponent<goddamnit.Physics>().restartOnDeath == ph.restartOnDeath) && ph.restartOnDeath;
        Spin sp = g[0].GetComponent<Spin>();
        spint.isOn = g.All(x => x.GetComponent<Spin>().isEnabled == sp.isEnabled) && sp.isEnabled;
        spin.text = g.All(x => x.GetComponent<Spin>().amount == sp.amount) ? sp.amount.ToString() : "-";

        //}
        for (int i = 0; i < g.Length; i++)
        {
            xPos[i] = g[i].transform.position.x;
            yPos[i] = g[i].transform.position.y;
            yScale[i] = g[i].transform.localScale.y;
            xScale[i] = g[i].transform.localScale.x;
            rot[i] = g[i].transform.localEulerAngles.z;
        }


        if (SamePosition(xPos)) posX.text = g[0].transform.position.x.ToString();
        else posX.text = "-";
        if (SamePosition(yPos)) posY.text = g[0].transform.position.y.ToString();
        else posY.text = "-";
        if (SamePosition(rot)) rotZ.text = g[0].transform.localEulerAngles.z.ToString();
        else rotZ.text = "-";
        if (SamePosition(xScale)) scaleX.text = g[0].transform.localScale.x.ToString();
        else scaleX.text = "-";
        if (SamePosition(yScale)) scaleY.text = g[0].transform.localScale.y.ToString();
        else scaleY.text = "-";

        ObjectComponent oc = m[0].GetComponent<ObjectComponent>();
        
        if (g.All(x => x.GetComponent<ObjectComponent>().obj == oc.obj))
        {
            textPanel.SetActive(false);
            turretPanel.SetActive(false);
            buttonPanel.SetActive(false);
            launchPadPanel.SetActive(false);
            tpPanel.SetActive(false);
            colorPanel.SetActive(false);
            chPanel.SetActive(false);
            
            switch (oc.obj)
            {
                case ObjectComponent.ObjectType.button:
                    buttonPanel.SetActive(true);

                    Button b = m[0].GetComponentInChildren<Button>();
                    b_offOnRestart.isOn = g.All(x => x.GetComponentInChildren<Button>().offOnRestart == b.offOnRestart) && b.offOnRestart;
                    b_turnBackOff.isOn = g.All(x => x.GetComponentInChildren<Button>().turnBackOff == b.turnBackOff) && b.turnBackOff;
                    b_immidiatelyActive.isOn = g.All(x => x.GetComponentInChildren<Button>().immediatelyActivate == b.immediatelyActivate) && b.immediatelyActivate;
                    break;
                case ObjectComponent.ObjectType.text:
                    textPanel.SetActive(true);
                    colorPanel.SetActive(true);
                    // Text script
                    TextComponent tc = m[0].GetComponent<TextComponent>();
                    tx_contents.text = g.All(x => x.GetComponent<TextComponent>().contents == tc.contents) ? tc.contents : "-";
                    tx_fontSize.text = g.All(x => x.GetComponent<TextComponent>().fontSize == tc.fontSize) ? tc.fontSize.ToString() : "-";
                    tx_bold.isOn = g.All(x => x.GetComponentInChildren<TextComponent>().bold == tc.bold) && tc.bold;
                    tx_italic.isOn = g.All(x => x.GetComponentInChildren<TextComponent>().italic == tc.italic) && tc.italic;

                    // Color script
                    ColorComponent cc = m[0].GetComponent<ColorComponent>();
                    col_enable.isOn = g.All(x => x.GetComponent<ColorComponent>().enabled == cc.enabled) && cc.enabled;
                    g_col_enable.isOn = g.All(x => x.GetComponent<ColorComponent>().g_enabled == cc.g_enabled) && cc.g_enabled;
                    float rccc = g.All(x => x.GetComponent<ColorComponent>().r == cc.r) ? cc.r : 255;
                    float gccc = g.All(x => x.GetComponent<ColorComponent>().g == cc.g) ? cc.g : 255;
                    float bccc = g.All(x => x.GetComponent<ColorComponent>().b == cc.b) ? cc.b : 255;
                    float accc = g.All(x => x.GetComponent<ColorComponent>().a == cc.a) ? cc.a : 255;
                    float g_rcc = g.All(x => x.GetComponent<ColorComponent>().g_r == cc.g_r) ? cc.g_r : 255;
                    float g_gcc = g.All(x => x.GetComponent<ColorComponent>().g_g == cc.g_g) ? cc.g_g : 255;
                    float g_bcc = g.All(x => x.GetComponent<ColorComponent>().g_b == cc.g_b) ? cc.g_b : 255;
                    float g_acc = g.All(x => x.GetComponent<ColorComponent>().g_a == cc.g_a) ? cc.g_a : 255;

                    g_picker.AssignColor(new Color(g_rcc / 255, g_gcc / 255, g_bcc / 255, g_acc / 255));
                    picker.AssignColor(new Color(rccc / 255, gccc / 255, bccc / 255, accc / 255));
                    break;
                case ObjectComponent.ObjectType.BlackHole:
                    tpPanel.SetActive(true);

                    tp_RecieverID.interactable = !ec.spawnPairObjects;
                    tp_WorkAsReciever.interactable = !ec.spawnPairObjects;

                    if (ec.spawnPairObjects)
                    {
                        tp_t_receiverID.color = disabledColor;
                        tp_t_workAsReceiver.color = disabledColor;
                    }
                    else
                    {
                        tp_t_receiverID.color = Color.white;
                        tp_t_workAsReceiver.color = Color.white;
                    }

                    TeleporterComponentScript tpc = m[0].GetComponent<TeleporterComponentScript>();
                    tp_WorkAsReciever.isOn = g.All(x => x.GetComponent<TeleporterComponentScript>().WorkAsReciever == tpc.WorkAsReciever) && tpc.WorkAsReciever;
                    tp_teleportObjects.isOn = g.All(x => x.GetComponent<TeleporterComponentScript>().teleportObjects == tpc.teleportObjects) && tpc.teleportObjects;
                    tp_RecieverTPBack.isOn = g.All(x => x.GetComponent<TeleporterComponentScript>().RecieverTPBack == tpc.RecieverTPBack) && tpc.RecieverTPBack;
                    tp_ResetYVelocity.isOn = g.All(x => x.GetComponent<TeleporterComponentScript>().KeepYVelocity == tpc.KeepYVelocity) && tpc.KeepYVelocity;
                    tp_RecieverID.text = g.All(x => x.GetComponent<TeleporterComponentScript>().RecieverID == tpc.RecieverID) ? tpc.RecieverID.ToString() : "-";
                    tp_forceX.text = g.All(x => x.GetComponent<TeleporterComponentScript>().force.x == tpc.force.x) ? tpc.force.x.ToString() : "-";
                    tp_forceY.text = g.All(x => x.GetComponent<TeleporterComponentScript>().force.y == tpc.force.y) ? tpc.force.y.ToString() : "-";
                    tp_RecieverTPBack.interactable = tp_WorkAsReciever.isOn;
                    
                    tp_t_RecieverTpBack.color = !tp_WorkAsReciever.isOn ? disabledColor : Color.white;
                    break;
                case ObjectComponent.ObjectType.turret:
                    turretPanel.SetActive(true);

                    Turret tr = m[0].GetComponent<Turret>();
                    t_followPlayer.isOn = g.All(x => x.GetComponent<Turret>().targetPlayer == tr.targetPlayer) && tr.targetPlayer;
                    t_cooldown.text = g.All(x => x.GetComponent<Turret>().cooldown == tr.cooldown) ? tr.cooldown.ToString() : "-";
                    t_bulletSpeed.text = g.All(x => x.GetComponent<Turret>().bulletSpeed == tr.bulletSpeed) ? tr.bulletSpeed.ToString() : "-";
                    t_offset.text = g.All(x => x.GetComponent<Turret>().offset == tr.offset) ? tr.offset.ToString() : "-";
                    break;
                case ObjectComponent.ObjectType.launchPad:
                    launchPadPanel.SetActive(true);

                    Launchpad lp = m[0].GetComponent<Launchpad>();
                    l_forceX.text = g.All(x => x.GetComponent<Launchpad>().x == lp.x) ? lp.x.ToString() : "-";
                    l_forceY.text = g.All(x => x.GetComponent<Launchpad>().y == lp.y) ? lp.y.ToString() : "-";
                    break;
                case ObjectComponent.ObjectType.platform:
                    colorPanel.SetActive(true);
                    ColorComponent cc1 = m[0].GetComponent<ColorComponent>();
                    col_enable.isOn = g.All(x => x.GetComponent<ColorComponent>().enabled == cc1.enabled) && cc1.enabled;
                    g_col_enable.isOn = g.All(x => x.GetComponent<ColorComponent>().g_enabled == cc1.g_enabled) && cc1.g_enabled;
                    float rc = g.All(x => x.GetComponent<ColorComponent>().r == cc1.r) ? cc1.r : 255;
                    float gc = g.All(x => x.GetComponent<ColorComponent>().g == cc1.g) ? cc1.g : 255;
                    float bc = g.All(x => x.GetComponent<ColorComponent>().b == cc1.b) ? cc1.b : 255;
                    float ac = g.All(x => x.GetComponent<ColorComponent>().a == cc1.a) ? cc1.a : 255;
                    float g_rc = g.All(x => x.GetComponent<ColorComponent>().g_r == cc1.g_r) ? cc1.g_r : 255;
                    float g_gc = g.All(x => x.GetComponent<ColorComponent>().g_g == cc1.g_g) ? cc1.g_g : 255;
                    float g_bc = g.All(x => x.GetComponent<ColorComponent>().g_b == cc1.g_b) ? cc1.g_b : 255;
                    float g_ac = g.All(x => x.GetComponent<ColorComponent>().g_a == cc1.g_a) ? cc1.g_a : 255;

                    picker.AssignColor(new Color(rc / 255, gc / 255, bc / 255, ac / 255));
                    g_picker.AssignColor(new Color(g_rc / 255, g_gc / 255, g_bc / 255, g_ac / 255));
                    break;
                case ObjectComponent.ObjectType.Checkpoint:
                    chPanel.SetActive(true);
                    Checkpoint ch = m[0].GetComponent<Checkpoint>();
                    c_enabled.isOn = g.All(x =>  x.GetComponent<Checkpoint>().isEnabled ==  ch.isEnabled) && ch.isEnabled;
                    c_priority.text = g.All(x => x.GetComponent<Checkpoint>().priority == ch.priority) ? ch.priority.ToString() : "-";
                    c_startPos.isOn = g.All(x => x.GetComponent<Checkpoint>().startPos == ch.startPos) && ch.startPos;
                    break;
                default:
                    textPanel.SetActive(false);
                    turretPanel.SetActive(false);
                    buttonPanel.SetActive(false);
                    launchPadPanel.SetActive(false);
                    tpPanel.SetActive(false);
                    colorPanel.SetActive(false);
                    break;
            }
        }
        else
        {
            textPanel.SetActive(false);
            turretPanel.SetActive(false);
            buttonPanel.SetActive(false);
            launchPadPanel.SetActive(false);
            tpPanel.SetActive(false);
            colorPanel.SetActive(false);
        }
    }
    public bool nuhUh;
    public void UpdateFields(Behaviour args = null)
    {
        if (nuhUh) return;
        if (ec.pstate == EditorControls.PlayState.stop)
            StartCoroutine(Updt(args));
        MainEditorComponent.Instance.beat = false;
    }

    IEnumerator Updt(Behaviour args = null)
    {
        yield return new WaitUntil(() => m != null);
        if (nuhUh) yield return null;
        // god is dead and i killed him
        for (int i = 0; i < m.Length; i++)
        {
            m[i].transform.position = new Vector3(posX.text != "-" ? float.Parse(posX.text, NumberStyles.Any) : m[i].transform.position.x,
                posY.text != "-" ? float.Parse(posY.text, NumberStyles.Any) : m[i].transform.position.y, 0);
            m[i].transform.localScale = new Vector3(scaleX.text != "-" ? float.Parse(scaleX.text, NumberStyles.Any) : m[i].transform.localScale.x,
                scaleY.text != "-" ? float.Parse(scaleY.text, NumberStyles.Any) : m[i].transform.localScale.y, m[i].transform.localScale.z);
            m[i].transform.localEulerAngles = new Vector3(0, 0, rotZ.text != "-" ? float.Parse(rotZ.text, NumberStyles.Any) : m[i].transform.localEulerAngles.z);
            Moving mo = m[i].GetComponent<Moving>();
            //mo.speed = float.Parse(speed.text, NumberStyles.Any);
            if (args == move) mo.isEnabled = move.isOn;
            //mo.wait = float.Parse(wait.text, NumberStyles.Any);
            //mo.to.x = float.Parse(toX.text, NumberStyles.Any);
            //mo.to.y = float.Parse(toY.text, NumberStyles.Any);
            
            //if (args == stickable) mo.stickable = stickable.isOn;
            if (args == loop) mo.loop = loop.isOn;
            if (args == goBack) mo.goBack = goBack.isOn;
            if (args == plus)
            {
                mo.points.Clear();
                mo.points = SetupCycle.instance.ConvertToPoints();
            }

            if (args == restart) mo.restartWhenDeath = restart.isOn;
            if (args == useTime) mo.useTime = useTime.isOn;

            Spin sp = m[i].GetComponent<Spin>();
            if (args == spint) sp.isEnabled = spint.isOn;
            if (args == spin) sp.amount = spin.text != "-" ? float.Parse(spin.text, NumberStyles.Any) : sp.amount;
            goddamnit.Physics ph = m[i].GetComponent<goddamnit.Physics>();
            if (args == physics) ph.isEnabled = physics.isOn;
            if (args == mass) ph.mass = mass.text != "-" ? float.Parse(mass.text, NumberStyles.Any) : ph.mass;
            if (args == lockX) ph.lockX = lockX.isOn;
            if (args == lockY) ph.lockY = lockY.isOn;
            if (args == lockZ) ph.lockZ = lockZ.isOn;
            if (args == restartOnDeath) ph.restartOnDeath = restartOnDeath.isOn;
            
            ph.Init();
            ObjectComponent oc = m[i].GetComponent<ObjectComponent>();
            if (m.All(x => x.GetComponent<ObjectComponent>().obj == m[0].GetComponent<ObjectComponent>().obj))
            {
                string[] miscSplit = oc.misc.Split('|');
                miscSplit[0] = $"{mo.isEnabled},{mo.restartWhenDeath},{mo.instantlyStart},{mo.loop},{mo.goBack},{mo.useTime},";

                foreach (Points point in mo.points)
                {
                    miscSplit[0] += $"{point.to.x}!{point.to.y}!{point.offset}!{point.speedortime}!{(int)point.ease}~";
                }
                
                miscSplit[0] += $",{sp.isEnabled},{sp.amount},{ph.isEnabled},{ph.mass}," +
                    $"{ph.lockX},{ph.lockY},{ph.lockZ},{ph.restartOnDeath}";
                
                oc.misc = miscSplit[0] + "|";
                switch (m[0].GetComponent<ObjectComponent>().obj)
                {
                    case ObjectComponent.ObjectType.button:
                        Button btn = m[i].GetComponentInChildren<Button>();
                        if (args == b_offOnRestart) btn.offOnRestart = b_offOnRestart.isOn;
                        if (args == b_offOnRestart) btn.smallBit.gameObject.SetActive(b_offOnRestart.isOn);
                        if (args == b_turnBackOff) btn.turnBackOff = b_turnBackOff.isOn;
                        if (args == b_immidiatelyActive) btn.immediatelyActivate = b_immidiatelyActive.isOn;
                        
                        m[i].GetComponent<ObjectComponent>().misc = miscSplit[0] + $"|{btn.offOnRestart},{btn.turnBackOff},{btn.immediatelyActivate}".ToLower();
                        
                        break;
                    case ObjectComponent.ObjectType.text:
                        TextComponent tc = m[i].GetComponent<TextComponent>();
                        // Text component
                        if (args == tx_contents) tc.ChangeText(tx_contents.text != "-" ? tx_contents.text : tc.contents);
                        if (args == tx_fontSize) tc.ChangeFontSize(tx_fontSize.text != "-" ? float.Parse(tx_fontSize.text, NumberStyles.Any) : tc.fontSize);
                        if (args == tx_bold) tc.bold = tx_bold.isOn;
                        if (args == tx_italic) tc.italic = tx_italic.isOn;

                        // Color component
                        ColorComponent cc = m[i].GetComponent<ColorComponent>();
                        if (args == g_col_enable) cc.g_enabled = g_col_enable.isOn;

                        if (args == g_picker)
                        {
                            cc.g_r = Mathf.RoundToInt(g_picker.CurrentColor.r * 255);
                            cc.g_g = Mathf.RoundToInt(g_picker.CurrentColor.g * 255);
                            cc.g_b = Mathf.RoundToInt(g_picker.CurrentColor.b * 255);
                            cc.g_a = Mathf.RoundToInt(g_picker.CurrentColor.a  * 255);
                        }

                        
                        if (args == col_enable) cc.enabled = col_enable.isOn;

                        if (args == picker)
                        {
                            cc.r = Mathf.RoundToInt(picker.CurrentColor.r * 255);
                            cc.g = Mathf.RoundToInt(picker.CurrentColor.g * 255);
                            cc.b = Mathf.RoundToInt(picker.CurrentColor.b * 255);
                            cc.a = Mathf.RoundToInt(picker.CurrentColor.a * 255);
                        }
                        
                        oc.misc = miscSplit[0] + $"|{tc.contents},{tc.fontSize},{cc.enabled},{cc.r},{cc.g},{cc.b},{cc.a},{cc.g_enabled},{cc.g_r},{cc.g_g},{cc.g_b},{cc.g_a},{tc.bold},{tc.italic}";
                        break;
                    case ObjectComponent.ObjectType.turret:
                        Turret tr = m[i].GetComponent<Turret>();
                        if (args == t_bulletSpeed) tr.bulletSpeed = t_bulletSpeed.text != "-" ? float.Parse(t_bulletSpeed.text, NumberStyles.Any) : tr.bulletSpeed;
                        if (args == t_cooldown) tr.cooldown = t_cooldown.text != "-" ? float.Parse(t_cooldown.text, NumberStyles.Any) : tr.cooldown;
                        if (args == t_offset) tr.offset = t_offset.text != "-" ? float.Parse(t_offset.text, NumberStyles.Any) : tr.offset;
                        if (args == t_followPlayer) tr.targetPlayer = t_followPlayer.isOn;
                        oc.misc = miscSplit[0] + $"|{tr.targetPlayer},{tr.cooldown},{tr.bulletSpeed},{tr.offset}".ToLower();
                        break;
                    case ObjectComponent.ObjectType.launchPad:
                        Launchpad lp = m[i].GetComponent<Launchpad>();
                        if (args == l_forceX) lp.x = l_forceX.text != "-" ? float.Parse(l_forceX.text, NumberStyles.Any) : lp.x;
                        if (args == l_forceY) lp.y = l_forceY.text != "-" ? float.Parse(l_forceY.text, NumberStyles.Any) : lp.y;
                        oc.misc = miscSplit[0] + $"|{lp.x},{lp.y}";
                        break;
                    case ObjectComponent.ObjectType.platform:
                        // Color component
                        ColorComponent ccc = m[i].GetComponent<ColorComponent>();
                        if (args == g_col_enable) ccc.g_enabled = g_col_enable.isOn;

                        if (args == g_picker)
                        {
                            ccc.g_r = Mathf.RoundToInt(g_picker.CurrentColor.r * 255);
                            ccc.g_g = Mathf.RoundToInt(g_picker.CurrentColor.g * 255);
                            ccc.g_b = Mathf.RoundToInt(g_picker.CurrentColor.b * 255);
                            ccc.g_a = Mathf.RoundToInt(g_picker.CurrentColor.a  * 255);
                        }

                        
                        if (args == col_enable) ccc.enabled = col_enable.isOn;

                        if (args == picker)
                        {
                            ccc.r = Mathf.RoundToInt(picker.CurrentColor.r * 255);
                            ccc.g = Mathf.RoundToInt(picker.CurrentColor.g * 255);
                            ccc.b = Mathf.RoundToInt(picker.CurrentColor.b * 255);
                            ccc.a = Mathf.RoundToInt(picker.CurrentColor.a * 255);
                        }
    
                        oc.misc = miscSplit[0] + $"|{ccc.enabled},{ccc.r},{ccc.g},{ccc.b},{ccc.a},{ccc.g_enabled},{ccc.g_r},{ccc.g_g},{ccc.g_b},{ccc.g_a}";
    
                        break;
                    case ObjectComponent.ObjectType.BlackHole:
                        TeleporterComponentScript tpc = m[i].GetComponent<TeleporterComponentScript>();
                        
                        if (!ec.spawnPairObjects && args == tp_WorkAsReciever) tpc.WorkAsReciever = tp_WorkAsReciever.isOn;
                        if (args == tp_RecieverTPBack) tpc.RecieverTPBack = tp_RecieverTPBack.isOn;
                        if (args == tp_ResetYVelocity) tpc.KeepYVelocity = tp_ResetYVelocity.isOn;
                        if (args == tp_teleportObjects) tpc.teleportObjects = tp_teleportObjects.isOn;
                        if (args == tp_RecieverID) tpc.RecieverID = tp_RecieverID.text != "-" ? int.Parse(tp_RecieverID.text) : tpc.RecieverID;
                        if (args == tp_forceX) tpc.force.x = tp_forceX.text != "-" ? float.Parse(tp_forceX.text, NumberStyles.Any) : tpc.force.x;
                        if (args == tp_forceY) tpc.force.y = tp_forceY.text != "-" ? float.Parse(tp_forceY.text, NumberStyles.Any) : tpc.force.y;
                        oc.misc = miscSplit[0] + $"|{tpc.RecieverID},{tpc.WorkAsReciever},{tpc.RecieverTPBack},{tpc.KeepYVelocity},{tpc.force.x},{tpc.force.y},{tpc.teleportObjects}";

                        break;
                    case ObjectComponent.ObjectType.Checkpoint:
                        Checkpoint ch = m[i].GetComponent<Checkpoint>();
                        if (args == c_enabled) ch.isEnabled = c_enabled.isOn;
                        if (args == c_priority) ch.priority = c_priority.text != "-" ? int.Parse(c_priority.text, NumberStyles.Any) : ch.priority;
                        if (args == c_startPos) ch.startPos = c_startPos.isOn;
                        
                        oc.misc = miscSplit[0] + $"|{ch.isEnabled},{ch.priority},{ch.startPos}";
                        break;
                    default:
                        break;
                }
            }
            
        }
    }

    bool SamePosition(float[] someObjects)
    {
        for (int i = 0; i < someObjects.Length - 1; i++)
        {
            if (someObjects[i] != someObjects[i + 1])
            {
                return false;
            }
        }
        return true;
    }
}
