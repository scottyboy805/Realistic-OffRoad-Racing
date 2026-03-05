using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RGSK.Extensions;
using TMPro;

namespace RGSK
{
    public class DemoHubScreen : UIScreen
    {
        [SerializeField] GameObject entryPrefab;
        [SerializeField] GameObject entryPrefabDummy;
        [SerializeField] ScrollRect demoScrollView;
        [SerializeField] ScrollRect showcaseScrollView;
        [SerializeField] Tab demoTab;
        [SerializeField] Tab showcaseTab;

        public override void Initialize()
        {
            foreach (var demo in DemoManager.Instance.demos)
            {
                CreateEntry(demo, demo.isShowcase);
            }

            if (entryPrefabDummy != null)
            {
                Instantiate(entryPrefabDummy, demoScrollView.content);
            }

            demoTab.startSelectable = demoScrollView.content.GetChild(0).gameObject;
            showcaseTab.startSelectable = showcaseScrollView.content.GetChild(0).gameObject;

            base.Initialize();
        }

        void CreateEntry(DemoManager.DemoDefinition demo, bool showcase)
        {
            var entry = Instantiate(entryPrefab, showcase ? showcaseScrollView.content : demoScrollView.content);
            var btn = entry.GetOrAddComponent<DemoLoaderButton>().id = demo.id;
            var nameText = entry.transform.Find("Bottom/Name_Text");
            var previewImage = entry.transform.Find("Preview/Preview_Image");

            if (nameText != null)
            {
                nameText.GetComponent<TMP_Text>()?.SetText(demo.name);
            }

            if (previewImage != null)
            {
                previewImage.GetComponent<Image>()?.SetSprite(demo.preview);
            }
        }
    }
}