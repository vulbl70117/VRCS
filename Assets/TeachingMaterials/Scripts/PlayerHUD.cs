using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    private Animator _Animator;

    [Header("Damage Indicator")]
    public RectTransform _IndicatorRotate;
    public Image _IndicatorImage;
    //public Transform _IndicatorHelperRoot;
    //public Transform _IndicatorHelper;
    public float _IndicatorVisibleTime = 5f;
    private float _IndicatorAlpha;
    private Vector3 _IndicatorLastPos;

    public Image _BloodyScreen;
    
    // Start is called before the first frame update
    void Start()
    {
        _Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHitIndicator();
    }
    
    public void DisplayShot(Vector3 from)
    {
        _IndicatorLastPos = from;

        _IndicatorAlpha = _IndicatorVisibleTime;
    }

    private void UpdateHitIndicator()
    {
        if (_IndicatorAlpha <= 0f)
            return;
        //_IndicatorHelperRoot.position = pb.transform.position;
        //_IndicatorHelperRoot.rotation = pb.transform.rotation;

            //_IndicatorHelper.LookAt(_IndicatorLastPos);
            if (_IndicatorAlpha > 0f)
            _IndicatorAlpha -= Time.deltaTime;

        _IndicatorImage.color = new Color(1f, 1f, 1f, _IndicatorAlpha);

        //Debug.DrawLine(transform.root.position, _IndicatorLastPos, Color.red);
        //Debug.Log(Quaternion.LookRotation(_IndicatorLastPos - transform.root.position).eulerAngles);
        _IndicatorRotate.localRotation = Quaternion.Euler(0, 0, Quaternion.LookRotation(_IndicatorLastPos - transform.root.position).eulerAngles.y);
        //_IndicatorRotate.localRotation = Quaternion.Euler(0f, 0f, -_IndicatorHelper.localEulerAngles.y);
    }

    public void DisplayBossAttack()
    {
        //_Animator.SetTrigger("BossAttack");
    }

    public void DisplayHurtState(float state, bool die = false)
    {
        //if (main.killAllZombies)
        //{
        //    _BloodyScreen.color = Color.clear;
        //    return;
        //}

        if (die)
            _BloodyScreen.color = Color.white;
        else if (state >= .5f)
            _BloodyScreen.color = Color.clear;
        else
            _BloodyScreen.color = new Color(1, 1, 1, 1-state);
    }
}
