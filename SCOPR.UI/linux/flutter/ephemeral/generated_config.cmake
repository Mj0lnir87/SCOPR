# Generated code do not commit.
file(TO_CMAKE_PATH "/home/rmeert/Development/flutter" FLUTTER_ROOT)
file(TO_CMAKE_PATH "/home/rmeert/Development/projects/sampleapp" PROJECT_DIR)

set(FLUTTER_VERSION "1.0.0+1" PARENT_SCOPE)
set(FLUTTER_VERSION_MAJOR 1 PARENT_SCOPE)
set(FLUTTER_VERSION_MINOR 0 PARENT_SCOPE)
set(FLUTTER_VERSION_PATCH 0 PARENT_SCOPE)
set(FLUTTER_VERSION_BUILD 1 PARENT_SCOPE)

# Environment variables to pass to tool_backend.sh
list(APPEND FLUTTER_TOOL_ENVIRONMENT
  "FLUTTER_ROOT=/home/rmeert/Development/flutter"
  "PROJECT_DIR=/home/rmeert/Development/projects/sampleapp"
  "DART_OBFUSCATION=false"
  "TRACK_WIDGET_CREATION=true"
  "TREE_SHAKE_ICONS=false"
  "PACKAGE_CONFIG=/home/rmeert/Development/projects/sampleapp/.dart_tool/package_config.json"
  "FLUTTER_TARGET=/home/rmeert/Development/projects/sampleapp/lib/main.dart"
)
