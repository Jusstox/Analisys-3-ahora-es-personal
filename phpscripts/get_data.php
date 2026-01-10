<?php
include 'db_connect.php';

function fetchTable($conn, $tableName) {
    if (!$conn) { return array(); }
    
    $sql = "SELECT * FROM $tableName";
    $result = $conn->query($sql);
    
    $rows = array();
    if ($result) {
        while($row = $result->fetch_assoc()) {
            $rows[] = $row;
        }
    }
    return $rows;
}

$sql_runs = "SELECT Game_Runs.*, Game_Sessions.Player_id 
             FROM Game_Runs 
             JOIN Game_Sessions ON Game_Runs.GameSession_ID = Game_Sessions.Session_id";

$result_runs = $conn->query($sql_runs);
$gameRuns = array();
if ($result_runs) {
    while($row = $result_runs->fetch_assoc()) {
        $gameRuns[] = $row;
    }
}

$gameSessions = fetchTable($conn, "Game_Sessions");

$positions = array();
$sql_pos = "SELECT * FROM Player_positions LIMIT 50000";
$result_pos = $conn->query($sql_pos);
if ($result_pos) {
    while($row = $result_pos->fetch_assoc()) {
        $positions[] = $row;
    }
}

$response = array(
    "gameRuns" => $gameRuns,
    "gameSessions" => $gameSessions,
    "positions" => $positions,
    "deaths" => fetchTable($conn, "Death_Event"),
    "kills" => fetchTable($conn, "Kill_Event"),
    "playerHits" => fetchTable($conn, "PlayerHit_Event"),
    "enemyHits" => fetchTable($conn, "EnemyHit_Event"),
    "checkpoints" => fetchTable($conn, "Checkpoint_Event"),
    "heals" => fetchTable($conn, "Heal_Event"),
    "breakBoxes" => fetchTable($conn, "BreakBox_Event"),
    "buttons" => fetchTable($conn, "Button_Event"),
    "keys" => fetchTable($conn, "Key_Event"),
    "jumps" => fetchTable($conn, "Jump_Event")
);

header('Content-Type: application/json');
echo json_encode($response);

$conn->close();
?>