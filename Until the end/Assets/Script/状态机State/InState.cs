using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public interface InState//接口
    //三种接口类型
    {
        void Enter();
        void Update();
        void Exit();
    }


    public class StateMachine //状态机
    {
        public InState currentState;
    //接口类 现在的状态
        private Dictionary<Type, InState> states = new Dictionary<Type, InState>();
    //字典
        public void AddState<T>(T state) where T : InState
            //添加状态  T  T的形参   T属于InState类
        {
             states[typeof(T)] = state;
        //         关键字
        }
        public T GetState<T>() where T : class, InState
        {
            return states[typeof(T)] as T;
        }
        public void Update()
        {
            currentState?.Update();
        }
        public void ChangeState<T>() where T:InState
        {
            if (states.TryGetValue(typeof(T), out InState newState))
            {
                currentState?.Exit();
                currentState = newState;
                currentState?.Enter(); 
            }
        }
    
    }

