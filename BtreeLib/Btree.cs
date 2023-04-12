using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BtreeLib
{
    public class Btree 
    {
        public int t;
        private Page _root;
        public int Count { get; private set; }

        public Btree(int t)
        {
            this.t = t;
            _root = new Page(true, t);
            Count = 0;
        }

        public bool Find(int findKey)
        {
            if (_root==null)
            {
                throw new Exception(); //искллючение что дерево пустое
            }
            var currentPage = _root;
            int i = 0;
            while (i != currentPage.KeyCount)
            {
                while (findKey > currentPage[i]) //находим нужную дочернюю страницу 
                {
                    i++;
                }
                if ((currentPage[i]!=0) && (findKey == currentPage[i])) // если сразу нашли ключ, то все хорошо
                {
                    return true;
                }
                if (currentPage.IsLeaf) // если прошли при этом все страницу - лист, то элемента нет
                {
                    return false;
                }
            
                if (i==currentPage.KeyCount)  //если нужно перейти на последнюю дочернюю страницу
                {
                    currentPage = currentPage._child[currentPage.KeyCount];
                    i = 0;
                }
                else //иначе спускаемся на предпоследнюю дочернюю и ищем снова
                {
                    currentPage = currentPage._child[i];
                    i = 0;
                }
            }
            return false;
        }

        public void Add(int addKey)
        {
            var currentPage = _root;
            int i = 0;
            //нашли нужное место для вставки или для перехода на нужную дочернюю страницу
            while (i != currentPage.KeyCount)
            {
                while (currentPage[i] < addKey)
                {
                    i++;
                }
                //если это лист, то сдвигаем все элементы справа на один и вставляем addKey
                if (currentPage.IsLeaf)
                {
                    for (int j = currentPage.KeyCount; j > i; j--)
                    {
                        currentPage[j] = currentPage[j - 1];
                    }
                    currentPage[i] = addKey;
                    currentPage.KeyCount++;
                }
                //иначе переходим на дочернюю страницу и начинаем поиск на ней
                else
                {
                    currentPage = currentPage._child[i];
                    i = 0;
                }
            }
            //после добавления элемента проверяем страницу на заполненность
            if (currentPage.KeyCount == 2 * t - 1)
            {
                SplitPage(currentPage);
            }
        }



        private void SplitPage( Page fullpage)
        {
            //находим средний элемент, который уйдет наверх
            var middleKey = fullpage[t-1];
            if (fullpage==_root) //для корня необходимо выполнить разделение только один раз
            {
                //в новый корень запишем только middleKey
                _root = new Page(false, t);
                _root[0] = middleKey;
                fullpage[t-1] = 0;  //сотрем его из старого места
                var RightPage = new Page(true, t);
                for (int i=0;i<t-1;i++)       //добавим еще одну страницу и запишем в нее все элементы правее среднего, при этом удалив их из старой страницы
                {
                    RightPage[i] = fullpage[i + t];
                    fullpage[i + t] = 0;
                }
                //настроим ссылки от нового корня
                _root._child[0] = fullpage;
                _root._child[1] =RightPage;
            }
            else
            {
                var RightPage = new Page(true, t); //добавим еще одну страницу и запишем в нее все элементы правее среднего, при этом удалив их из старой страницы
                for (int i = 0; i < t-1; i++)
                {
                    RightPage[i] = fullpage[i + t];
                    fullpage[i + t] = 0;
                }
                var Parent = fullpage._parent;
                //найдем среди ключей родителя нужно место для вставки middleKey
                int j = 0;
                while ((middleKey > Parent[j]) && (j<Parent.KeyCount))
                {
                    j++;
                }
                //передвинем все элементы до места вставки на один вправо, чтобы освободить место для вставки middleKey
                for (int i=Parent.KeyCount; i>j;i--)
                {
                    Parent[i] = Parent[i - 1];
                }
                //вставим его при этом удалив в старой странице
                Parent[j] = middleKey;
                fullpage[t-1] = 0;
                //добвим ссылку на новую RightPage
                Parent._child[j+1]=RightPage;
                if (Parent.KeyCount==2*t-1) //если опять нужно разделить страницу
                {
                    SplitPage(Parent);
                }                
            }
        }
        
    }
}
