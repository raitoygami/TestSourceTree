using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UILittleMaze : MonoBehaviour
{
    // Start is called before the first frame update
    public Slider mSliderWidth;
    public Slider mSliderHeight;

    public Text mTextWidth;
    public Text mTextHeight;

    public Button mButtonStart;

    private int mWidth = 20;
    private int mHeight = 20;

    private void Awake()
    {
        mSliderWidth.onValueChanged.AddListener(
            (value) => {
                mTextWidth.text = value.ToString();
                mWidth = (int)value;
            });
        mSliderWidth.minValue = 15;
        mSliderWidth.maxValue = 50;
        mSliderWidth.value = mWidth;
        
        mSliderHeight.onValueChanged.AddListener(
            (value) => {
                mTextHeight.text = value.ToString();
                mHeight = (int)value;
            });
        mSliderHeight.minValue = 15;
        mSliderHeight.maxValue = 50;
        mSliderHeight.value = mHeight;

        mButtonStart.onClick.AddListener(() => {
            LittleMaze.Instance().GameStart(mWidth, mHeight);
        });
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
