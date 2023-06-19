#!/bin/bash

repo=git@github.com:takashicompany/merge-game.git
remote=merge-game
branch=master
path=Assets/takashicompany/Game/Merge

if [ $# -eq 0 ]; then
	echo "this is git-subtree-suppot tool.\noptions:\n init\n pull\n push"
	exit 1
fi

if [ $1 = init ]; then
	git remote add ${remote} ${repo}
	git fetch ${remote}
	git subtree add --prefix=${path} ${remote} ${branch}
elif [ $1 = pull ] ; then
	git subtree pull --prefix=${path} ${remote} ${branch}
elif [ $1 = push ] ; then
	git subtree push --prefix=${path} ${remote} ${branch}
elif [ $1 = split ] ; then
	git subtree split --prefix=${path} -b $2
else
	echo "$1 option not found"
fi