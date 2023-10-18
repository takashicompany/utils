#!/bin/bash

repo=git@github.com:takashicompany/stack-match-game.git
remote=stack-match-game
branch=master
path=Assets/takashicompany/Game/StackMatch
file_pattern="*subtree.sh"

if [ $# -eq 0 ]; then
    echo "This is git-subtree-support tool.\nOptions:\n init\n pull\n push\n push_subtree_sh"
    exit 1
fi

if [ $1 = init ]; then
    git remote add ${remote} ${repo}
    git fetch ${remote}
    git subtree add --prefix=${path} ${remote} ${branch}
elif [ $1 = pull ] ; then
    git subtree pull --prefix=${path} ${remote} ${branch}
elif [ $1 = push ] ; then
    if git show-ref --quiet --verify refs/remotes/${remote}/${branch}; then
        git subtree push --prefix=${path} ${remote} ${branch}
    else
        echo "Subtree is not registered. Please run 'init' option first."
    fi
elif [ $1 = pushall ] ; then
    find . -maxdepth 1 -type f -name "${file_pattern}" -exec sh {} push \;
else
    echo "$1 option not found"
fi