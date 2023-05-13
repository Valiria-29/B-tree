using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BtreeLib
{
    public class Btree <T> where T : IComparable
    {
        public int MinTreeDegree;
        private Page<T> _root;
        public int Count;

        IComparer <T> _comparer;
        public Btree(int minTreeDegree)
        {
            this.MinTreeDegree = minTreeDegree;
            _root = new Page<T>(true, minTreeDegree);
            _comparer = Comparer<T>.Default;
            
        }
        public Btree(int minTreeDegree, IComparer<T> comparer)
        {
            MinTreeDegree = minTreeDegree;
            _root = new Page<T>(true, minTreeDegree);
            _comparer = comparer;
        }

        public bool Find(T findKey)
        {
            if (_root==null)
            {
                throw new Exception(); //искллючение что дерево пустое
            }
            var currentPage = _root;
            int i = 0;
            while (i != currentPage.KeyCount)
            {
                while ( i<currentPage.KeyCount && _comparer.Compare(findKey, currentPage[i])>0 ) //находим нужную дочернюю страницу 
                {
                    i++;
                }
                if ((!currentPage[i].Equals(default(T))) && (_comparer.Compare(findKey ,currentPage[i])==0)) // если сразу нашли ключ, то все хорошо
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

        public void Add(T addKey)
        {
            
            var currentPage = _root;
            int i = 0;
            //нашли нужное место для вставки или для перехода на нужную дочернюю страницу
            while (i <= currentPage.KeyCount)
            {

                while (!currentPage[i].Equals(default(T)) && (_comparer.Compare(currentPage[i], addKey) < 0))
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
                    Count++;
                    break;
                }
                //иначе переходим на дочернюю страницу и начинаем поиск на ней
                else
                {
                    var parentreference = currentPage;
                    currentPage = currentPage._child[i];
                    currentPage._parent = parentreference;
                    i = 0;

                }
            }
            //после добавления элемента проверяем страницу на заполненность
            if (currentPage.KeyCount == 2 * MinTreeDegree - 1)
            {
                SplitPage(currentPage);
            }
        }



        private void SplitPage( Page<T> fullpage)
        {
            //находим средний элемент, который уйдет наверх
            var middleKey = fullpage[MinTreeDegree-1];
            if (fullpage==_root) //для корня необходимо выполнить разделение только один раз
            {
                //в новый корень запишем только middleKey
                _root = new Page<T>(false, MinTreeDegree);
                _root[0] = middleKey;
                _root.KeyCount++;
                fullpage[MinTreeDegree - 1] = default(T);   //сотрем его из старого места
                fullpage.KeyCount--;                  
                var RightPage = new Page<T>(true, MinTreeDegree);
                fullpage._parent = _root;
                RightPage._parent= _root;
                for (int i=0;i<MinTreeDegree-1;i++)       //добавим еще одну страницу и запишем в нее все элементы правее среднего, при этом удалив их из старой страницы
                {
                    RightPage[i] = fullpage[i + MinTreeDegree];
                    RightPage.KeyCount++;
                    fullpage[i + MinTreeDegree] = default(T);
                    fullpage.KeyCount--;
                }
                //настроим ссылки от нового корня
                _root._child[0] = fullpage;
                _root._child[1] =RightPage;
            }
            else
            {
                var RightPage = new Page<T>(true, MinTreeDegree); //добавим еще одну страницу и запишем в нее все элементы правее среднего, при этом удалив их из старой страницы
                RightPage._parent = fullpage._parent;
                for (int i = 0; i < MinTreeDegree-1; i++)
                {
                    RightPage[i] = fullpage[i + MinTreeDegree];
                    RightPage.KeyCount++;
                    fullpage[i + MinTreeDegree] = default(T);
                    fullpage.KeyCount--;
                }
                var Parent = fullpage._parent;
                //найдем среди ключей родителя нужно место для вставки middleKey
                int j = 0;
                while ((_comparer.Compare(middleKey , Parent[j])>0) && (j<Parent.KeyCount))
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
                Parent.KeyCount++;
                fullpage[MinTreeDegree-1] = default(T);
                fullpage.KeyCount--;
                //добвим ссылку на новую RightPage
                Parent._child[j+1]=RightPage;
                if (Parent.KeyCount==2*MinTreeDegree-1) //если опять нужно разделить страницу
                {
                    SplitPage(Parent);
                }                
            }
        }


        public int BinSearch<T> (Page<T> currentPage, T value) where T : IComparable
        {
            var left = 0;
            var right = currentPage.KeyCount;
            if (left== right)
            {
                return left;
            }
            while (true)
            {
                if (right - left == 1)
                {
                    if (currentPage[left].CompareTo(value) == 0)
                        return left;
                    if (currentPage[right].CompareTo(value) == 0)
                        return right;
                    return -1;
                }
                else
                {
                    var middle = left + (right - left) / 2;
                    var comparisonResult = currentPage[middle].CompareTo(value);
                    if (comparisonResult == 0)
                        return middle;
                    if (comparisonResult < 0)
                        left = middle;
                    if (comparisonResult > 0)
                        right = middle;
                }
            }
        }
        
    }
}
