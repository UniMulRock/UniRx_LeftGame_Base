using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using System.Collections.Generic;

namespace Utility.System
{
    public class SceneManager : SingletonMonoBehaviour<SceneManager>
    {
        //遷移にかかる時間
        public const float TRANS_SECOND = 0.5f;

        public enum STATE
        {
            BLANK,
            TITLE,
            GAME,
        }

        Dictionary<STATE, string> SceneName = new Dictionary<STATE, string>
        {
            { STATE.BLANK, "Blank"},
            { STATE.TITLE, "Title"},
            { STATE.GAME, "Game" },
        };

        public STATE state { get; set; }

        public static IReactiveProperty<STATE> state_change;

        new void Awake()
        {
            base.Awake();

            DontDestroyOnLoad(gameObject);

            state = STATE.TITLE;

            state_change = this.UpdateAsObservable()
                .Select(_ => state)
                .DistinctUntilChanged()
                .ToReactiveProperty();
        }

        void Update()
        {
            //Android用処理
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
                Debug.Log("input escape");
            }
        }

        //状態遷移
        public static void ChangeState(STATE st)
        {
            Instance._ChangeState(st);
        }

        void _ChangeState(STATE st)
        {
            StartCoroutine(TransState(st));
        }

        IEnumerator TransState(STATE st)
        {
            Resources.UnloadUnusedAssets();

            //フェードアウト
            yield return StartCoroutine(FadeManager.Instance.FadeOut(TRANS_SECOND / 2));

            state = st;
            Debug.Log("Scene to " + state);

            //memori release
            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName[STATE.BLANK]);

            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName[state]);

            //フェードインを待つ
            yield return StartCoroutine(FadeManager.Instance.FadeIn(TRANS_SECOND / 2));
        }
    }
}
