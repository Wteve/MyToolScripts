using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace MyTool {

    public class LongPressButton : Button, IPointerDownHandler, IPointerUpHandler {

        private bool mIsDown;
        //是否按下private 
        private float mCheckTime = 0.5f;
        //按下判断时间
        private float mDownTime;

        //按下触发间隔
        private float mCurTime = 0;
        private float mClickTime = 0.1f;

        private float checkQuickTime = 2.0f;
        private float quickTime = 0.01f;

        public Action stopPress;

        private int actionTimes = 0;

        protected override void OnEnable() {
            base.OnEnable();
        }

        protected override void OnDisable() {
            base.OnDisable();
            ResetState();
        }

        public void ResetState() {
            if (!mIsDown)
                return;

            mDownTime = 0;
            mCurTime = 0;
            mIsDown = false;

            stopPress?.Invoke();
        }

        public override void OnPointerClick(PointerEventData eventData) {
            if (actionTimes <= 0) {
                base.OnPointerClick(eventData);
            }
        }

        public override void OnPointerDown(PointerEventData eventData) {
            base.OnPointerDown(eventData);
            if (!interactable)
                return;
            mDownTime = Time.time;
            mCurTime = 0;
            mIsDown = true;

            actionTimes = 0;
        }

        public override void OnPointerUp(PointerEventData eventData) {
            base.OnPointerUp(eventData);

            ResetState();
        }

        public override void OnPointerExit(PointerEventData eventData) {
            base.OnPointerExit(eventData);

            ResetState();
        }

        private void Update() {
            if (!interactable)
                return;
            if (mIsDown) {
                float spown = Time.time - mDownTime;
                if (spown > mCheckTime) {

                    mCurTime += Time.deltaTime;
                    if (mCurTime >= (spown > checkQuickTime ? quickTime : mClickTime)) {
                        mCurTime = 0;
                        onClick?.Invoke();

                        actionTimes++;
                    }
                }
            }
        }
    }

}