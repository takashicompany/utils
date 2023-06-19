#!/bin/bash

repo=git@github.com:takashicompany/battle-game.git
remote=battle-game
branch=master
path=Assets/takashicompany/Game/Battle

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
else
	echo "$1 option not found"
fi