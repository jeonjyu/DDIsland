/// <summary>
/// 도감 시스템 공용 enum 모음
/// </summary>

// 대분류 탭
public enum MainTab
{
    Quest,      // 퀘스트
    Journal     // 도감
}

// 도감 소분류 카테고리
public enum JournalCategory
{
    Fish,       // 어종
    Costume,    // 코스튬
    Interior,   // 인테리어
    Album,     // 음반     
    Food       // 음식
}

// 도감 필터링 옵션 (정렬 드롭다운)
public enum CollectionFilter
{
    All,        // 전체 (기본값)
    Owned,      // 등록 (IsUnlocked = TRUE)
    NotOwned    // 미등록 (IsUnlocked = FALSE)
}
