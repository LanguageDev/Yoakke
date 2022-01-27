// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Collections.Trees;

namespace Yoakke.Collections.Fuzzer;

internal class BstTreeNode : TreeNodeBase<BstTreeNode>
{
    public int Height { get; set; } = 1;

    public BstTreeNode(int key)
        : base(key)
    {
    }
}

internal class BstTreeSet : TreeSetBase<BstTreeNode>
{
    public override bool Insert(int k)
    {
        var insertion = BinarySearchTree.Insert(
            root: this.root,
            key: k,
            keySelector: n => n.Key,
            keyComparer: Comparer<int>.Default,
            makeNode: k => new(k));
        if (insertion.Existing is not null) return false;
        this.root = insertion.Root;
        ++this.Count;
        return true;
    }

    public override bool Delete(int k)
    {
        var search = BinarySearchTree.Search(
            root: this.root,
            key: k,
            keySelector: n => n.Key,
            keyComparer: Comparer<int>.Default);
        if (search.Found is null) return false;
        this.root = BinarySearchTree.Delete(root: this.root, search.Found);
        --this.Count;
        return true;
    }
}
