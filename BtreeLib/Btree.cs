using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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

        private (Page<T> , int ) Find(T findKey)
        {
            if (_root.KeyCount == 0) 
            {
                throw new Exception("Empty b-tree"); //искллючение что дерево пустое
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
            if (Find(containsKey).Item2 == -1)
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


        #region [SplitPage]
        private void SplitPage( Page<T> fullpage)
        {
            //находим средний элемент, который уйдет наверх
            var middleKey = fullpage[MinTreeDegree-1];
            if (fullpage==_root) //для корня необходимо выполнить разделение только один раз
            {
                //в новый корень запишем только middleKey
               var newRoot = new Page<T>(false, MinTreeDegree);
                newRoot[0] = middleKey;
                newRoot.KeyCount++;
                fullpage[MinTreeDegree - 1] = default(T);   //сотрем его из старого места
                fullpage.KeyCount--;                  
                var RightPage = new Page<T>(true, MinTreeDegree);
                fullpage._parent = newRoot;
                RightPage._parent= newRoot;
                for (int i=0;i<MinTreeDegree-1;i++)       //добавим еще одну страницу и запишем в нее все элементы правее среднего, при этом удалив их из старой страницы
                {
                    RightPage[i] = fullpage[i + MinTreeDegree];
                    RightPage.KeyCount++;
                    fullpage[i + MinTreeDegree] = default(T);
                    fullpage.KeyCount--;
                }
                for (int i=0;i<MinTreeDegree;i++)
                {
                    
                    RightPage._child[i] = fullpage._child[i + MinTreeDegree];
                    if (RightPage._child[i] != null)
                    {
                        RightPage.IsLeaf = false;
                    }
                    fullpage._child[i + MinTreeDegree] = null;
                }

                //переопределим новый корень
                _root = newRoot;
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
                    Parent._child[i + 1] = Parent._child[i];
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
        #endregion

        #region [BinSearch]
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
        #endregion
         
        public void Delete ( T deleteKey)
        {
            var (page, index) = Find(deleteKey);
            if (page.Equals(default(Page<T>)))
            {
                throw new Exception("This key is no in b-tree");
                //исключение что нет такого элемента в дереве
            }
            else //если элемент в дереве нашелся, то проверяем где именно он находится
            {
               
                if (page.IsLeaf)
                {
                    DeleteFromLeaf(page, index );
                }
                else
                {
                    DeleteFromInternalNode(deleteKey , page , index);
                }
            }
        }

        #region [DeleteFromLeaf]
        //метод удаления элемента с листа
        private void DeleteFromLeaf( Page<T> page , int index)
        {
            if (page._parent == null) //если удаляем из корня
            {
                ShiftKey(page, index);
                Count--;
                page.KeyCount--;
                return;
            }
            if (page.KeyCount - 1 >= MinTreeDegree-1) //если после удаления элементов на странице будет допустимое значение(обычное удаление)
            {
                page[index]= default(T);
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
        #endregion

        #region [ShiftKey]
        //метод, который сдвигает все элементы, после удаляемого, на один влево
        private void ShiftKey(Page<T> page , int deleteKeyIndex)
        {
            page[deleteKeyIndex]= default(T);
            for (int i = deleteKeyIndex; i < page.KeyCount-1; i++)
            {
                page[i] = page[i + 1];
            }
            page[page.KeyCount-1] = default(T);
        }
        #endregion

        #region[BorrowFromNeighbor]
        private void BorrowFromNeighbor (Page<T> page, T deleteKey)
        {
           //найдем  индекс родителя в родительской странице, чтобы перейти к соседям
           var parentPage= page._parent;
            int parentIndex = 0;
            for (int i = 0; i < parentPage.KeyCount; i++)
            {
                if (parentPage[i].CompareTo(deleteKey)>0)
                {
                    parentIndex = i;
                    break;
                }
                parentIndex = parentPage.KeyCount;
            }
            Page<T> rightNeighborPage = null;
            Page<T> leftNeighborPage = null;
            if (parentIndex != 0)
            {
                leftNeighborPage = parentPage._child[parentIndex - 1];
            }
            if (parentIndex != 2 * MinTreeDegree - 1)
            {
                if (parentIndex != parentPage.KeyCount)
                {
                    rightNeighborPage = parentPage._child[parentIndex + 1];
                }
            }
            //если правый сосед есть и у него достаточно элементов чтобы поделиться
            if (rightNeighborPage!=null && rightNeighborPage.KeyCount > MinTreeDegree-1)
            {
                BorrowFromRightNeighbor(page, rightNeighborPage, parentPage, parentIndex);
                return;
            }
            //если левый сосед есть и у него достаточно элементов чтобы поделиться
            if (leftNeighborPage!=null && leftNeighborPage.KeyCount > MinTreeDegree-1)
            {
                BorrowFromLeftNeighbor(page, leftNeighborPage, parentPage, parentIndex);
                return;
            }
            //если нужно объединять c правым соседом
            if (rightNeighborPage != null)
            {
                UniteWhithRightNeighbors(page, rightNeighborPage, parentPage, parentIndex);
                return;
            }
            //если нужно объединять c левым соседом
            if (leftNeighborPage != null)
            {
                UniteWhithLeftNeighbors(page, leftNeighborPage, parentPage, parentIndex);
            }    

        }
        #endregion

        #region[BorrowFromNeighbor вспомогательные методы]
        private void BorrowFromRightNeighbor (Page<T> page, Page<T> rightNeighborPage, Page<T> parentPage, int parentIndex )
        {
            ////поставим найденный элемент из родительской страницы на первое свободное место
            page[page.KeyCount] = parentPage[parentIndex];
            //на его место поставим самого левого из правого соседа
            parentPage[parentIndex] = rightNeighborPage[0];
            //сдвинем в правом соседе все на один влево
            ShiftKey(rightNeighborPage, 0);
            rightNeighborPage.KeyCount--;
            rightNeighborPage[rightNeighborPage.KeyCount] = default(T);
            page.KeyCount++;
        }

        private void BorrowFromLeftNeighbor(Page<T> page, Page<T> leftNeighborPage, Page<T> parentPage, int parentIndex)
        {
            parentIndex = parentIndex - 1;
            //сдвинем все элементы на странице на один вправо, чтобы освободить место для земены
            for (int i =page.KeyCount; i>0; i--)
            {
                page[i] = page[i - 1]; 
            }
            ////поставим найденный элемент из родительской страницы на нулевое место
            page[0] = parentPage[parentIndex];
            //на его место поставим самого правого из левого соседа
            parentPage[parentIndex] = leftNeighborPage[leftNeighborPage.KeyCount-1];
            leftNeighborPage.KeyCount--;
            leftNeighborPage[leftNeighborPage.KeyCount] = default(T);
            page.KeyCount++;
        }

        private void UniteWhithRightNeighbors (Page<T> page , Page<T> rightNeighborPage, Page<T> parentPage, int parentIndex)
        {
            int shift = page.KeyCount + 1;
            //сдвинем все элементы в правой странице на величину сдвига
            for (int i = rightNeighborPage.KeyCount - 1; i >= 0; i--)
            {
                rightNeighborPage[rightNeighborPage.KeyCount + shift - 1] = rightNeighborPage[i];
                rightNeighborPage.KeyCount++;
            }
            //спустим родителя
            rightNeighborPage[shift-1]= parentPage[parentIndex];
            parentPage._child[parentIndex] = null;
            parentPage[parentIndex]=default(T);
            parentPage.KeyCount--;
            //перенесем все элементы в правую страницу

            for (int i =0; i<page.KeyCount;i++)
            {
                rightNeighborPage[i] = page[i];
            }
            //сотрем page
            page = default   (Page<T>);
            parentPage._child[parentIndex] = rightNeighborPage;
            parentPage._child[parentIndex + 1] = null;
            rightNeighborPage._parent= parentPage;
            //Если у родителя не осталось элементов, нужно поднять все на уровень выше
            if (parentPage.KeyCount == 0)
            {
                //если обнулился корень
                if (parentPage._parent == null)
                {
                    _root = rightNeighborPage;
                    _root._child = rightNeighborPage._child;
                    _root.IsLeaf = rightNeighborPage.IsLeaf;
                    _root._parent = null;
                }
            }
        }
        private void UniteWhithLeftNeighbors(Page<T> page, Page<T> leftNeighborPage, Page<T> parentPage, int parentIndex)
        {
            //спускаем родителя
            leftNeighborPage[leftNeighborPage.KeyCount] = parentPage[parentIndex-1];
            leftNeighborPage.KeyCount++;
           
            //переносим в левого соседа все ключи
            for (int i=0;i<page.KeyCount;i++)
            {
                leftNeighborPage[leftNeighborPage.KeyCount]= page[i];
                leftNeighborPage.KeyCount++;
            }
            parentPage._child[parentIndex] = null;
            for (int i=parentIndex-1;i<parentPage.KeyCount-1; i++)
            {
                parentPage[i] = parentPage[i + 1];
                parentPage._child[i] = parentPage._child[i + 1];
            }
            parentPage[parentPage.KeyCount-1] = default;
            parentPage._child[parentPage.KeyCount] = null;
            parentPage.KeyCount--;
            //Если у родителя не осталось элементов, нужно поднять все на уровень выше
            if (parentPage.KeyCount==0)
            {
                //если обнулился корень
                if (parentPage._parent==null)
                {
                    _root = leftNeighborPage;
                    _root._child = leftNeighborPage._child;
                    _root.IsLeaf = leftNeighborPage.IsLeaf;
                    _root._parent=null;

                }
            }



        }
        #endregion

        private void DeleteFromInternalNode (T deleteKey, Page<T> page, int index)
        {
            var leftNeighborPage = page._child[index];
            var rightNeighborPage = page._child[index+1];
            var substituteKey = default(T);
            bool KeyIsDelete = false;
            if (leftNeighborPage != default(Page<T>) && leftNeighborPage.KeyCount > MinTreeDegree - 1)//можно найти заместителя в левом поддереве
            {
                substituteKey = FindSubstituteKeyInLeftSubTree(leftNeighborPage, deleteKey, index);//метод нахождения самого правого элемента в поддереве
                page[index] = substituteKey;
                KeyIsDelete = true;
            }
            if (rightNeighborPage != default(Page<T>) && rightNeighborPage.KeyCount > MinTreeDegree - 1 && !KeyIsDelete)// можно найти заместителя в правом поддереве
            {
                substituteKey= FindSubstituteKeyInRightSubTree(rightNeighborPage, deleteKey, index);//метод нахождения самого левого элемента в поддереве
                page[index] = substituteKey;
                KeyIsDelete = true;
            }
            if (leftNeighborPage == default(Page<T>) && rightNeighborPage == default(Page<T>) && !KeyIsDelete)
            {
                //метод обычного удаления элемента из страницы (у него нет дочерних узлов)
                page[index]=default(T);
                page.KeyCount--;
                Count--;
                KeyIsDelete = true;
                
            }
            if (!KeyIsDelete && substituteKey.CompareTo(default(T)) == 0) //нужен метод объединения страниц (эти страницы листовые, иначе они бы имели более, чем t-1 элемент)
            {
                UnionTwoPage(leftNeighborPage, rightNeighborPage, page, index, page._parent);
            }
        }

        private T FindSubstituteKeyInLeftSubTree (Page<T> leftNeighborPage, T deleteKey, int index) 
        {
            //переходим на самую левую подстраницу, пока не дойдем до листа
            var currentPage = leftNeighborPage;
            T currentSubstituteKey;
            if (currentPage.IsLeaf != true)
            {
                while (currentPage._child[currentPage.KeyCount].IsLeaf != true)
                {
                    currentPage = currentPage._child[currentPage.KeyCount];
                }
            }
            //как только дошли до листа выбираем самый правый элемент
            currentSubstituteKey = currentPage[currentPage.KeyCount - 1];
            // удаляем его
            currentPage[currentPage.KeyCount - 1] = default(T);
            currentPage.KeyCount--;
            Count--;
            
            return currentSubstituteKey;

        }
        private T FindSubstituteKeyInRightSubTree(Page<T>  rightNeighborPage, T deleteKey, int index)
        {
            //переходим на самую правую подстраницу, пока не дойдем до листа
            var currentPage = rightNeighborPage;
            T currentSubstituteKey;
            if (currentPage.IsLeaf != true)
            {
                while (currentPage._child[0].IsLeaf != true)
                {
                    currentPage = currentPage._child[0];
                }
            }
            //как только дошли до листа выбираем самый левый элемент
            currentSubstituteKey = currentPage[0];
            // удаляем его, смещая все остальные на один влево
            for (int i=0;i<currentPage.KeyCount-1; i++)
            {
                currentPage[i] = currentPage[i + 1];
            }
            currentPage[currentPage.KeyCount-1] = default(T);
            currentPage.KeyCount--;
            Count--;
            return currentSubstituteKey;
        }
        private void UnionTwoPage (Page<T> leftNeighborPage, Page<T> rightNeighborPage , Page<T> deleteKeyPage, int deleteKeyIndex, Page<T> parentPage)
        {
            //перенесем из правой все элементы в левую
            for ( int i=0; i< rightNeighborPage.KeyCount; i++)
            {
                leftNeighborPage[i + leftNeighborPage.KeyCount] = rightNeighborPage[i];
            }
            leftNeighborPage.KeyCount += rightNeighborPage.KeyCount;
            //стираем правую страницу
            rightNeighborPage = default;
            //в странице удаляемого элемента сдвинем все элементы после удаляемого на один влево, аналогично сдвинем ссылки на детей
            for ( int i = deleteKeyIndex;i<deleteKeyPage.KeyCount-1;i++)
            {
                deleteKeyPage[i] = deleteKeyPage[i + 1];
                deleteKeyPage._child[i + 1] = deleteKeyPage._child[i + 2];
            }
            deleteKeyPage[deleteKeyPage.KeyCount-1] = default;
            deleteKeyPage._child[deleteKeyPage.KeyCount] = default;
            deleteKeyPage.KeyCount--;
            Count--;
            //Если у родителя не осталось элементов, нужно поднять наверх страницу
            if (deleteKeyPage.KeyCount == 0)
            {
                //если обнулился корень
                if (deleteKeyPage._parent == null)
                {
                    _root = leftNeighborPage;
                    _root._child = leftNeighborPage._child;
                    _root.IsLeaf = leftNeighborPage.IsLeaf;
                    _root._parent = null;
                }
                else
                {
                    //находим на каком месте среди детей родителя стояла page
                    int index = 0;
                    for( int i=0; i< parentPage._child.Length;i++)
                    {
                        if (parentPage._child[i]==deleteKeyPage)
                        {
                            index = i;
                            break;
                        }
                    }
                    parentPage._child[index] = leftNeighborPage;
                    leftNeighborPage._parent = parentPage;
                    parentPage._child[index].IsLeaf= leftNeighborPage.IsLeaf;
                    parentPage._child[index]._child = leftNeighborPage._child;
                }
            }

        }
    }
}
