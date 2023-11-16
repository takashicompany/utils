#!/bin/bash

# スクリプトのファイル名からパスと拡張子を除外
filename=$(basename "$0" ".sh")

# ファイル名が正しい形式であるかチェック（@で4つのパーツに分割できること）
IFS='@' read -ra ADDR <<< "$filename"
if [ ${#ADDR[@]} -ne 4 ]; then
    echo "エラー: ファイル名が不正です。正しい形式：cherry-pick@<checkout_branch>@<push_remote>@<push_remote_branch>.sh"
    exit 1
fi

checkout_branch=${ADDR[1]}
push_remote=${ADDR[2]}
push_remote_branch=${ADDR[3]}

echo "checkout_branch: $checkout_branch"
echo "push_remote: $push_remote"
echo "push_remote_branch: $push_remote_branch"

# 現在のブランチを保存
current_branch=$(git rev-parse --abbrev-ref HEAD)

# リモートリポジトリの情報を更新
git fetch $push_remote

# checkout_branchがローカルに存在するかチェック
if git show-ref --verify --quiet refs/heads/$checkout_branch; then
    echo "cherry-pickとpushを行います。"

    # 存在する場合: cherry-pickとpushを実行

    # 最後のコミットハッシュを取得
    last_commit=$(git rev-parse HEAD)

    # checkout_branchにチェックアウト
    git checkout $checkout_branch

    # cherry-pickしてpush_remote_branchにpush
    git cherry-pick $last_commit
    git push $push_remote $checkout_branch:$push_remote_branch
else
    echo "ローカルにブランチ $checkout_branch が存在しないため、$push_remote の $push_remote_branch からチェックアウトします。"

    # 存在しない場合: push_remoteからpush_remote_branchをcheckout_branchとしてチェックアウト
    git checkout -b $checkout_branch $push_remote/$push_remote_branch
fi

# 元のブランチに再度チェックアウト
git checkout $current_branch