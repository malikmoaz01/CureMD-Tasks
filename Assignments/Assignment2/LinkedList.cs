using System;

class Node
{
    public int data;
    public Node next;

    public Node(int data)
    {
        this.data = data;
        this.next = null;
    }
}

class LinkedList
{
    private Node head;

    public void InsertAtEnd(int data)
    {
        Node newNode = new Node(data);
        if (head == null)
        {
            head = newNode;
            return;
        }

        Node current = head;
        while (current.next != null)
        {
            current = current.next;
        }
        current.next = newNode;
    }

    public void InsertAtBeginning(int data)
    {
        Node newNode = new Node(data);
        newNode.next = head;
        head = newNode;
    }

    public void InsertAtAnyPosition(int data, int position)
    {
        if (position <= 0)
        {
            InsertAtBeginning(data);
            return;
        }

        Node newNode = new Node(data);
        Node current = head;
        int count = 0;

        while (current != null && count < position - 1)
        {
            current = current.next;
            count++;
        }

        if (current == null)
        {
            InsertAtEnd(data);
            return;
        }

        newNode.next = current.next;
        current.next = newNode;
    }

    public void Delete(int data)
    {
        if (head == null) return;

        if (head.data == data)
        {
            head = head.next;
            return;
        }

        Node current = head;
        while (current.next != null && current.next.data != data)
        {
            current = current.next;
        }

        if (current.next != null)
        {
            current.next = current.next.next;
        }
    }

    public void DeleteAtAnyPosition(int position)
    {
        if (head == null || position < 0) return;

        if (position == 0)
        {
            head = head.next;
            return;
        }

        Node current = head;
        int count = 0;

        while (current.next != null && count < position - 1)
        {
            current = current.next;
            count++;
        }

        if (current.next != null)
        {
            current.next = current.next.next;
        }
    }

    public void Display()
    {
        Node current = head;
        while (current != null)
        {
            Console.Write(current.data + " ");
            current = current.next;
        }
        Console.WriteLine();
    }
}

class Program
{
    static void Main(string[] args)
    {
        LinkedList list = new LinkedList();
        list.InsertAtEnd(10);
        list.InsertAtEnd(20);
        list.InsertAtEnd(30);
        list.InsertAtAnyPosition(15, 1);
        list.InsertAtBeginning(5);
        list.Delete(30);
        list.DeleteAtAnyPosition(2);
        list.Display();
    }
}
