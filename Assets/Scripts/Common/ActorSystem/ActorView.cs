using System.Collections.Generic;
using Common.Graphics;
using UnityEngine;

namespace Common.ActorSystem
{
    /// <summary>
    /// The ActorView class represents an entity with various animations.
    /// A state refers to a continious animations 
    /// An action refers to an an animation with a finite duration
    /// An actor always maintains a state, and if it has an ongoing action that completes, the animation returns to the state animation.
    /// Action and state names must match the name of the animation
    /// </summary>
    public class ActorView : MonoBehaviour
    {
        [field: SerializeField] public List<ActorViewAction> Actions { get; private set; } = new List<ActorViewAction>();
        [field: SerializeField] public List<ActorViewState> States { get; private set; } = new List<ActorViewState>();
        [field: SerializeField] public SpritesAnimationDatas AnimationsDatas { get; private set; }

        public ActorViewAction CurrentAction => _currentAction;
        public ActorViewState CurrentState => _currentState;

        public int CurrentStateID => _currentState.ID;

        /// <summary>
        /// Time since the beginning of the current state
        /// </summary>
        public float StateTimer { get; private set; }

        [SerializeField] private SpriteRenderer _spriteRenderer;

        ActorViewAction _currentAction;
        ActorViewState _currentState;
        SpritesAnimationDatas.AnimationDatas _currentAnimation;
        float _timerCurrentAction;
        int _currentFrame;

        #region Initialize

        private void Awake()
        {
            if (_spriteRenderer == null)
                throw new System.Exception("_spriteRenderer is null");

            BuildStatesID();
        }

        /// <summary>
        /// Generates a unique index for each state based on the order in the list
        /// </summary>
        private void BuildStatesID()
        {
            int index = 0;
            foreach (var state in States)
            {
                state.ID = index;
                index++;
            }
        }

        private void Start()
        {
            PlayState(0);

            _currentAnimation = GetAnimation(_currentState.Name);
            _currentFrame = Random.Range(0, _currentAnimation.Sprites.Count);
            _spriteRenderer.sprite = _currentAnimation.Sprites[_currentFrame];
        }

        #endregion

        #region Update

        public void Update()
        {
            StateTimer += Time.deltaTime;

            UpdateAction();
            UpdateView();
        }

        private void UpdateAction()
        {
            if (_currentAction != null && _currentAnimation != null)
            {
                _timerCurrentAction += Time.deltaTime;

                if (_timerCurrentAction >= (1.0f / _currentAnimation.FPS) * (float)_currentAnimation.Sprites.Count)
                {
                    _timerCurrentAction = 0;

                    ActorViewAction lastAction = _currentAction;
                    EndAction();

                    string nextAction = lastAction.NextAction;
                    if (string.IsNullOrEmpty(nextAction) == false)
                        PlayAction(nextAction);
                    else
                        _currentAnimation = GetAnimation(_currentState.Name);
                }
            }
        }

        /// <summary>
        /// Updates the sprite according to the current state of the animation
        /// </summary>
        void UpdateView()
        {
            if (_currentAction != null)
            {
                int frame = (int)(_timerCurrentAction * _currentAnimation.FPS);

                if (frame >= _currentAnimation.Sprites.Count)
                    frame = _currentAnimation.Sprites.Count - 1;

                if (frame != _currentFrame)
                {
                    _currentFrame = frame;
                    _spriteRenderer.sprite = _currentAnimation.Sprites[_currentFrame];
                }

            }
            else if (_currentState != null)
            {
                int frame = (int)(((StateTimer * _currentAnimation.FPS)) % _currentAnimation.Sprites.Count);

                if (frame != _currentFrame)
                {
                    _currentFrame = frame;

                    if (_currentAnimation.Sprites.Count > _currentFrame && _currentFrame >= 0)
                    {
                        _spriteRenderer.sprite = _currentAnimation.Sprites[_currentFrame];
                    }
                }
            }
        }

        void EndAction()
        {
            _currentAction = null;
        }

        #endregion

        #region Tools

        public void PlayState(int stateID)
        {
            _currentFrame = -1;
            StateTimer = 0;

            _currentState = States[stateID];
            _currentAnimation = GetAnimation(_currentState.Name);
        }

        public void PlayState(string stateName)
        {
            ActorViewState state = GetState(stateName);

            if (state == null)
            {
                throw new System.Exception($"can't find state {stateName}");
            }
            else
            {
                PlayState(state.ID);
            }

        }

        public void PlayAction(string actionName)
        {
            ActorViewAction action = GetAction(actionName);
            if (action == null)
            {
                throw new System.Exception($"can't find action {actionName}");
            }

            SpritesAnimationDatas.AnimationDatas animation = GetAnimation(action.Name);

            if (animation == null)
            {
                throw new System.Exception($"can't find animation {action.Name}");
            }

            _currentAction = action;
            _timerCurrentAction = 0;
            _currentAnimation = GetAnimation(action.Name);
        }

        public bool HasState(string name)
        {
            foreach (var item in States)
            {
                if (item.Name == name)
                    return true;
            }

            return false;
        }

        public bool HasAction()
        {
            return _currentAction != null;
        }

        #endregion

        ActorViewState GetState(string name)
        {
            foreach (var item in States)
            {
                if (item.Name == name)
                    return item;
            }

            return null;
        }
        ActorViewAction GetAction(int index)
        {
            if (index < 0 || index >= Actions.Count)
                return null;

            return Actions[index];
        }
        ActorViewAction GetAction(string actionName)
        {
            foreach (var action in Actions)
            {
                if (action.Name == actionName)
                    return action;
            }
            return null;
        }

        SpritesAnimationDatas.AnimationDatas GetAnimation(string name)
        {
            return AnimationsDatas.GetAnimation(name);
        }
    }
}