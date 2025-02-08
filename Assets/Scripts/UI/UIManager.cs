using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField]
        private Page initialPage;
    
        private Stack<Page> pageStack;

        public int PageCount { get=> pageStack.Count; }

        private void Awake() {
            pageStack = new Stack<Page>();
        }
        private void Start()
        {
            if (initialPage != null)
            {
                PushPage(initialPage);
            }
        }

        public bool IsPageInStack(Page page)
        {
            return pageStack.Contains(page);
        }

        public bool IsPageOnTopOfStack(Page page)
        {
            return pageStack.Count > 0 && page == pageStack.Peek();
        }

        public void PushPage(Page page)
        {
            if(page == null)
            {
                Debug.LogError("Page is null!");
                return;
            }
            
            if(!page.gameObject.activeInHierarchy)
            {
                page.gameObject.SetActive(true);
            }
            page.Enter();

            if (pageStack.Count > 0)
            {
                Page currentPage = pageStack.Peek();

                if (currentPage.exitOnNewPagePush)
                {
                    currentPage.Exit();
                }
            }

            pageStack.Push(page);
        }

        public void PopPage()
        {
            if (pageStack.Count > 1)
            {
                Page page = pageStack.Pop();
                page.Exit();

                Page newCurrentPage = pageStack.Peek();
                if (newCurrentPage.exitOnNewPagePush)
                {
                    newCurrentPage.Enter();
                }
            }
            else
            {
                Debug.LogWarning("Trying to pop a page but only 1 page remains in the stack!");
            }
        }

        public void PopAllPages()
        {
            for (int i = 1; i < pageStack.Count; i++)
            {
                PopPage();
            }
        }
    }
}
