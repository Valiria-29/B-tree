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

        public (Page<T> , int ) Find(T findKey)
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
                    return (currentPage, i);
                }
                if (currentPage.IsLeaf) // если прошли при этом все страницу - лист, то элемента нет
                {
                    return (default(Page<T>), -1);
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
            return (default(Page<T>), -1);
        }


        public bool Contains (T containsKey)
        {
            if (Find(containsKey).Equals(default(T)))
            {
                return false;
            }
            else
            {
                return true;
            }
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


        //public int BinSearch<T> (Page<T> currentPage, T value) where T : IComparable
        //{
        //    var left = 0;
        //    var right = currentPage.KeyCount;
        //    if (left== right)
        //    {
        //        return left;
        //    }
        //    while (true)
        //    {
        //        if (right - left == 1)
        //        {
        //            if (currentPage[left].CompareTo(value) == 0)
        //                return left;
        //            if (currentPage[right].CompareTo(value) == 0)
        //                return right;
        //            return -1;
        //        }
        //        else
        //        {
        //            var middle = left + (right - left) / 2;
        //            var comparisonResult = currentPage[middle].CompareTo(value);
        //            if (comparisonResult == 0)
        //                return middle;
        //            if (comparisonResult < 0)
        //                left = middle;
        //            if (comparisonResult > 0)
        //                right = middle;
        //        }
        //    }
        //}
        
        public void Delete ( T deleteKey)
        {
            var (page, index) = Find(deleteKey);
            if (page.Equals(default(Page<T>)))
            {
                //исключение что нет такого элемента в дереве
            }
            else //если элемент в дереве нашелся, то проверяем где именно он находится
            {
                if (page.IsLeaf)
                {
                    DeleteFromList( page, index );
                }
                else
                {
                    DeleteFromNode(deleteKey , page , index);
                }
            }
        }
        //метод удаления элемента с листа
        public void DeleteFromList( Page<T> page , int index)
        {
            if (page._parent == null) //если удаляем из корня
            {
                ShiftKey(page, index);
                Count--;
                page.KeyCount--;
                return;
            }
            if (page.KeyCount - 1 >= MinTreeDegree-1) //если после удаления элементов на странице будет допустимое значение
            {
                ShiftKey(page, index);
                Count--;
                page.KeyCount--;
                return;
            }
            else //если после удаления станет меньше t-1 элемента, нужно занять из соседней страницы один элемент через родителя
            {
                var deleteKey = page[index];
                ShiftKey(page, index); //перенесли все элементы после удаляемого на один влево
                Count--;
                page.KeyCount--;
                BorrowFromNeighbor(page,deleteKey);

            }

        }
        //метод, который сдвигает все элементы, после удаляемого, на один влево
        public void ShiftKey(Page<T> page , int index)
        {
            for (int i = index; i < page.KeyCount-1; i++)
            {
                page[i] = page[i + 1];
            }
        }

        public void BorrowFromNeighbor (Page<T> page, T deleteKey)
        {
           //найдем родителя
           var parentpage= page._parent;
            int index = 0;
            for (int i = 0; i < parentpage.KeyCount; i++)
            {
                if (parentpage[i].CompareTo(deleteKey)>0)
                {
                    index = i;
                    break;
                }
            }
            //если у правого соседа достаточно элементов чтобы поделиться
            if (parentpage._child[index+1].KeyCount>MinTreeDegree)
            {
                //поставим найденный элемент из родительской страницы на первое свободное место
                page[page.KeyCount] = parentpage[index];
                parentpage[index] = parentpage._child[index + 1][0]; //на его место поставим самого левого из правого соседа
                //сдвинем в правом соседе все на один влево
                ShiftKey(parentpage._child[index + 1], 0);
                parentpage._child[index + 1].KeyCount--;
                parentpage._child[index + 1][parentpage._child[index + 1].KeyCount] = default(T);

            }
            //если у левого соседа достаточно элементов чтобы поделиться
            if (parentpage._child[index].KeyCount > MinTreeDegree)
            {
                //поставим найденный элемент из родительской страницы на первое свободное место
                page[page.KeyCount] = parentpage[index];
                parentpage[index] = parentpage._child[index][parentpage._child[index].KeyCount-1]; //на его место поставим самого правого из левого соседа
                //самый правый из левого соседа обнулим
                parentpage._child[index][parentpage._child[index].KeyCount - 1]= default(T);
                parentpage._child[index].KeyCount--;
            }
            //если нужно объединять
            else
            {

            }

           
           
        }

    }
}
